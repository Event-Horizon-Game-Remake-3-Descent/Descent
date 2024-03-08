using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //public InputManager inputManager;

    public float mouseSensitivity = 25f;
    public Transform mesh;
    public float XSpeed;
    public float YSpeed;
    public float CamSpeed;
    public float PlayerSpeed;
    public AnimationCurve DecelerationCurve;
    public float DecelerationSpeed;
    public float BankingSpeed;
    public float PitchingSpeed;
    public float PlayerMaxSpeed;
    Coroutine Coroutine;
    
    
    Rigidbody Rb;
    float X = 0; // up and down mov
    float Y = 0; // left and right mov

    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        
        Cursor.lockState = CursorLockMode.Locked; //TODO: da spostare
        
    }

    private void OnEnable()
    {
        InputManager.InputMap.Overworld.Movement.canceled += Decelerate;
        InputManager.InputMap.Overworld.Movement.started += StopSlowDownCycle;
        InputManager.InputMap.Overworld.VerticalMovement.canceled += Decelerate;
        InputManager.InputMap.Overworld.VerticalMovement.started += StopSlowDownCycle;

    }

    private void OnDisable()
    {
        InputManager.InputMap.Overworld.Movement.canceled -= Decelerate;
        InputManager.InputMap.Overworld.Movement.started -= StopSlowDownCycle;
        InputManager.InputMap.Overworld.VerticalMovement.canceled -= Decelerate;
        InputManager.InputMap.Overworld.VerticalMovement.started -= StopSlowDownCycle;
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
             
            
            Rb.AddForce(direction.y * Rb.transform.forward* PlayerSpeed * Time.fixedDeltaTime, ForceMode.VelocityChange);
            Rb.AddForce(direction.x * Rb.transform.right * PlayerSpeed * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }

        if (InputManager.IsBanking(out Vector3 banking))
        {
            Quaternion deltaRotation = Quaternion.Euler(0,0, banking.z * BankingSpeed * Time.deltaTime);
            Rb.MoveRotation(Rb.rotation*deltaRotation);
        }

        if (InputManager.IsMovingVertically(out Vector2 verticaldirection))
        {
            if (Coroutine != null) { StopCoroutine(Coroutine); }
            Rb.AddForce(verticaldirection.y * Rb.transform.up * PlayerSpeed * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }

        if (InputManager.IsPitching( out Vector2 pitching))
        {
            
            Quaternion pitchRotation = Quaternion.Euler(pitching.x * PitchingSpeed * Time.fixedDeltaTime, 0, 0);
            Quaternion smoothRot = Quaternion.Slerp(Rb.rotation, pitchRotation, 0);
            Rb.MoveRotation(Rb.rotation * pitchRotation);
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
        

        
           


        







   
        
        



            


       


            

        

            
               
    


        

}

