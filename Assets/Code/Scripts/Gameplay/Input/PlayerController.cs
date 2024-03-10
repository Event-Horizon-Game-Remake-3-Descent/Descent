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

    public float mouseSensitivity = 25f;
    public float BaseMouseSensitivity = 250f;
    public float GamepadSensMultiplier = 10f;
    public Transform mesh;
    public float XSpeed;
    public float YSpeed;
    public float CamSpeed;
    public float PlayerSpeed;
    public AnimationCurve DecelerationCurve;
    public float DecelerationSpeed;
    public float BankingSpeed;
    public bool IsBanking;
    public float PitchingSpeed;
    public float PlayerMaxSpeed;
    public bool UsingGamepad;
    Coroutine Coroutine;
    IEnumerator currentSnapCoroutine;
    public float SnapSpeed = 10;
    public float hp = 100f;
    
    

    Rigidbody Rb;
    float X = 0; // up and down mov
    float Y = 0; // left and right mov

    public float HP { get ; set ; }

    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        HP = hp;
    }

    private void Start()
    {
        
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
        



    }

    private void OnDisable()
    {
        InputManager.InputMap.Overworld.Movement.canceled -= Decelerate;
        InputManager.InputMap.Overworld.Movement.started -= StopSlowDownCycle;
        InputManager.InputMap.Overworld.VerticalMovement.canceled -= Decelerate;
        InputManager.InputMap.Overworld.VerticalMovement.started -= StopSlowDownCycle;
        InputManager.InputMap.Overworld.MouseX.started -= CheckTypeOfDevice;
        InputManager.InputMap.Overworld.MouseY.started -= CheckTypeOfDevice;
    }

    private void Update()
    {
       
    }
    private void FixedUpdate()
    {
        
        float mouseX = InputManager.InputMap.Overworld.MouseX.ReadValue<float>() * mouseSensitivity *Time.fixedDeltaTime ;
        float mouseY = InputManager.InputMap.Overworld.MouseY.ReadValue<float>() * mouseSensitivity * Time.fixedDeltaTime ;
        Rb.velocity = Vector3.ClampMagnitude(Rb.velocity, PlayerMaxSpeed);
        X = Mathf.SmoothDamp(X, mouseY, ref XSpeed, CamSpeed); 
        Y = Mathf.SmoothDamp(Y, mouseX, ref YSpeed, CamSpeed);


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
            Quaternion smoothRot = Quaternion.Slerp(Rb.rotation, pitchRotation, 0);
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
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            this.TakeDamage(50);
        }
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

