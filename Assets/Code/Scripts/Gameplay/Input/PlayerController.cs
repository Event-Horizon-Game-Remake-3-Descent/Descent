using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour , IDamageable
{
    //public InputManager inputManager;
    public delegate void GamePad();
    public static event GamePad OnGamePad;
    public static event GamePad OnKeyBoard;
    public delegate void PlayerState();
    public static event PlayerState OnPlayerDead;
    public delegate void PlayerReady(PlayerController Player);
    public static event PlayerReady OnPlayerReady;
    public delegate void PlayerCollecting();
    public static event PlayerCollecting OnUpdatingUiCollect;
    public static Action UpdatePlayerSens;

    private float mouseSensitivity = 25f;
    private float XSpeed;
    private float YSpeed;
    [Header("Sensibility")]
    [SerializeField] float BaseMouseSensitivity = 250f;
    [SerializeField] float GamepadSensMultiplier = 10f;
    [SerializeField] float CamSpeedSmoother;
    [Space]
    [Header("Player Parameters")]
    [SerializeField] float PlayerSpeed;
    [SerializeField] AnimationCurve DecelerationCurve;
    [SerializeField] float DecelerationSpeed;
    [SerializeField] float BankingSpeed;
    [SerializeField] float SnapSpeed = 10;
    [SerializeField] float PitchingSpeed;
    [SerializeField] float PlayerMaxSpeed;
    [SerializeField] float hp = 100f;
    [SerializeField] private float DamageOnCollion;
    [Space]
    [Header("Don't Touch This, it's just for debug")]
    public bool UsingGamepad;
    public bool IsBanking;
    Coroutine Coroutine;
    IEnumerator currentSnapCoroutine;
    Rigidbody Rb;
    float X = 0; // up and down mov
    float Y = 0; // left and right mov
    public float HP { get ; set ; }
    
    



    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        HP = hp;
        BaseMouseSensitivity = GameManager.MouseSens;
    }

    private void Start()
    {
        OnPlayerReady(this);
        mouseSensitivity = BaseMouseSensitivity;
        InputManager.InputMap.Overworld.Movement.canceled += Decelerate;
        InputManager.InputMap.Overworld.Movement.started += StopSlowDownCycle;
        InputManager.InputMap.Overworld.VerticalMovement.canceled += Decelerate;
        InputManager.InputMap.Overworld.VerticalMovement.started += StopSlowDownCycle;
        InputManager.InputMap.Overworld.MouseX.started += CheckTypeOfDevice;
        InputManager.InputMap.Overworld.MouseY.started += CheckTypeOfDevice;
    }
        

    private void OnEnable()
    {
        ShieldCollectible.OnShieldTaken += Healing;
        UpdatePlayerSens += ChangePlayerSensitivity;
    }




    private void OnDisable()
    {
        InputManager.InputMap.Overworld.Movement.canceled -= Decelerate;
        InputManager.InputMap.Overworld.Movement.started -= StopSlowDownCycle;
        InputManager.InputMap.Overworld.VerticalMovement.canceled -= Decelerate;
        InputManager.InputMap.Overworld.VerticalMovement.started -= StopSlowDownCycle;
        InputManager.InputMap.Overworld.MouseX.started -= CheckTypeOfDevice;
        InputManager.InputMap.Overworld.MouseY.started -= CheckTypeOfDevice;
        ShieldCollectible.OnShieldTaken -= Healing;
    }

    
    private void FixedUpdate()
    {
        
        float mouseX = InputManager.InputMap.Overworld.MouseX.ReadValue<float>() * mouseSensitivity *Time.fixedDeltaTime ;
        float mouseY = InputManager.InputMap.Overworld.MouseY.ReadValue<float>() * mouseSensitivity * Time.fixedDeltaTime ;
        Rb.velocity = Vector3.ClampMagnitude(Rb.velocity, PlayerMaxSpeed);
        X = Mathf.SmoothDamp(X, mouseY, ref XSpeed, CamSpeedSmoother); 
        Y = Mathf.SmoothDamp(Y, mouseX, ref YSpeed, CamSpeedSmoother);


        Quaternion rotation = Quaternion.Euler(-X * Time.fixedDeltaTime, Y * Time.fixedDeltaTime, 0); // asse y invertito con (-x)
        
        
        Rb.MoveRotation(Rb.rotation * rotation);

        if (InputManager.IsMoving(out Vector2 direction)) 
        {
            if (Coroutine != null) { StopCoroutine(Coroutine); }
             
            
            Rb.AddForce(direction.y * Rb.transform.forward* PlayerSpeed * Time.fixedDeltaTime, ForceMode.Impulse);
            Rb.AddForce(direction.x * Rb.transform.right * PlayerSpeed * Time.fixedDeltaTime, ForceMode.Impulse);
        }

        if (InputManager.IsBanking(out Vector3 banking))
        {
            IsBanking = true;  

           
            float zRotation = banking.z * BankingSpeed * Time.fixedDeltaTime;
            Quaternion deltaRotation = Quaternion.Euler(0, 0, zRotation);
            Quaternion newRotation = Rb.rotation * deltaRotation;
            Rb.MoveRotation(newRotation);

            
            float currentZAngle = NormalizeAngle(newRotation.eulerAngles.z);
            int targetZAngle = DetermineSnapTargetAngle(currentZAngle);

            
            if (currentSnapCoroutine != null)
            {
                StopCoroutine(currentSnapCoroutine);
            }
            currentSnapCoroutine = SnapToAngle(targetZAngle);
            StartCoroutine(currentSnapCoroutine);
        }
        else
        {
            IsBanking = false;  
        }

        if (InputManager.IsMovingVertically(out Vector2 verticaldirection))
        {
            if (Coroutine != null) { StopCoroutine(Coroutine); }
            Rb.AddForce(verticaldirection.y * Rb.transform.up * PlayerSpeed * Time.fixedDeltaTime, ForceMode.Impulse);
        }

        if (InputManager.IsPitching(out Vector2 pitching))
        {

            Quaternion pitchRotation = Quaternion.Euler(pitching.x * PitchingSpeed * Time.fixedDeltaTime, 0, 0);
            //Quaternion smoothRot = Quaternion.Slerp(Rb.rotation, pitchRotation, 0);
            Rb.MoveRotation(Rb.rotation * pitchRotation);
        }

    }

    void CheckTypeOfDevice(InputAction.CallbackContext used)
    {
        var usedDevice = used.control;
        if (usedDevice.device is Gamepad && UsingGamepad == false) 
        {
            mouseSensitivity *= GamepadSensMultiplier; 
            UsingGamepad = true;
            OnGamePad();
        }
        else if (usedDevice.device is Mouse || usedDevice.device is Keyboard)
        {
            mouseSensitivity = BaseMouseSensitivity;
            UsingGamepad = false;
            OnKeyBoard();
        }
        

    }

        

    void Decelerate(InputAction.CallbackContext obj )
    {
        Coroutine = StartCoroutine(SlowDown());
    }
        

    IEnumerator SlowDown()
    {
        Vector3 initialVelocity = Rb.velocity;
        float time = 0;
        
        while (time < 1) 
        {
            Rb.velocity = DecelerationCurve.Evaluate(time) * initialVelocity;
            time += DecelerationSpeed * Time.fixedDeltaTime ; 
            yield return null;
        }
        Rb.velocity = Vector3.zero;
    }
    void StopSlowDownCycle(InputAction.CallbackContext obj )
    {
        StopAllCoroutines();
    }




    float NormalizeAngle(float angle)
    {
        angle %= 360;
        if (angle > 180)
            return angle - 360;
        return angle;
    }

    int DetermineSnapTargetAngle(float currentZAngle)
    {
        int baseAngle = Mathf.FloorToInt(currentZAngle / 90) * 90;
        int nextAngle = baseAngle + 90;
        int prevAngle = baseAngle - 90;

        if (currentZAngle - baseAngle >= 31) return nextAngle;
        if (baseAngle - currentZAngle >= 31) return prevAngle;

        return baseAngle;
    }

    IEnumerator SnapToAngle(int targetAngle)
    {
        while (Mathf.Abs(NormalizeAngle(Rb.rotation.eulerAngles.z) - targetAngle) > 0.01f)
        {
            float currentZAngle = NormalizeAngle(Rb.rotation.eulerAngles.z);
            float newZAngle = Mathf.MoveTowardsAngle(currentZAngle, targetAngle, SnapSpeed * Time.fixedDeltaTime);
            Rb.MoveRotation(Quaternion.Euler(Rb.rotation.eulerAngles.x, Rb.rotation.eulerAngles.y, newZAngle));
            yield return new WaitForFixedUpdate();
        }
        currentSnapCoroutine = null;  
    }

    public void TakeDamage(float Damage)
    {   
        HP -= Damage;
        if (HP <= 0) { OnPlayerDead(); }
        OnUpdatingUiCollect();
    }

    void Healing(float value)
    {
        HP += value;
        OnUpdatingUiCollect();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            damageable.TakeDamage(DamageOnCollion);
            
        }
    }

    public void ChangePlayerSensitivity()
    {
        BaseMouseSensitivity = GameManager.MouseSens;
    }

    //void StartSnapCoroutine(int targetZAngle, float currentZAngle)
    //{
    //    if (Mathf.Abs(currentZAngle - targetZAngle) > 0.01f && (currentSnapCoroutine == null || !IsBanking))
    //    {
    //        if (currentSnapCoroutine != null)
    //        {
    //            StopCoroutine(currentSnapCoroutine);
    //        }
    //        currentSnapCoroutine = SnapToAngle(targetZAngle);
    //        StartCoroutine(currentSnapCoroutine);
    //    }
    //}
































}

