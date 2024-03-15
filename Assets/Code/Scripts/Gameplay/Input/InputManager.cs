using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static Action IsPaused;
    public static Action Resume = () => { };
    public delegate void OnShoot();
    public static event OnShoot OnPrimaryCalled = ()=> { };
    public static event OnShoot OnSecondaryCalled = () => { };
    public static event OnShoot OnLaunchingBomb = () => { };
    public delegate void OnPause();
    public static event OnPause OnPauseMenu = () => { };
    public delegate void CameraSelection();
    public static event CameraSelection OnRearCamera;
    public static event CameraSelection OnPlayerCamera;

    public static InputMap InputMap;

    bool AlreadyOnMenu;
    bool PlayerIsAlive;
    

    public static Vector2 MovementInput => InputMap.Overworld.Movement.ReadValue<Vector2>();
    public static Vector3 BankingInput => InputMap.Overworld.Banking.ReadValue<Vector3>();
    public static Vector2 VerticalMovement => InputMap.Overworld.VerticalMovement.ReadValue<Vector2>();
    public static Vector2 PitchingInput => InputMap.Overworld.Pitching.ReadValue<Vector2>();

    


   


    private void Awake()
    {
        InputMap = new InputMap();
        Application.targetFrameRate = 60;
        Cursor.lockState = CursorLockMode.Locked;
        PlayerIsAlive = true;

    }

    private void OnEnable()
    {
        InputMap.Enable();
        InputMap.Menu.Navigation.Disable();
        InputMap.Overworld.ShootPrimary.performed += TriggerPrimary;
        InputMap.Overworld.ShootSecondary.performed += TriggerSecondary;
        InputMap.Overworld.LaunchingBomb.performed += LaunchBomb;
        InputMap.Menu.Pause.performed += Paused;
        PlayerController.OnPlayerDead += PlayerIsDead;
        //IsPaused += () => AlreadyOnMenu = true;
        Resume += () => { AlreadyOnMenu = false; Cursor.lockState = CursorLockMode.Locked; };

        }

    private void TriggerPrimary(InputAction.CallbackContext f)
    {
        StartCoroutine(PrimaryPressed());
    }
    
    private void TriggerSecondary(InputAction.CallbackContext f)
    {
        StartCoroutine(SecondaryPressed());
    }

    private IEnumerator PrimaryPressed()
    {
        while(InputMap.Overworld.ShootPrimary.IsPressed())
        {
            OnPrimaryCalled();
            yield return null;
        }
    }

    private IEnumerator SecondaryPressed()
    {
        while (InputMap.Overworld.ShootSecondary.IsPressed())
        {
            OnSecondaryCalled();
            yield return null;
        }
    }

    private void LaunchBomb(InputAction.CallbackContext bomb)
    {
        if (InputMap.Overworld.LaunchingBomb.IsPressed())
        {
            OnLaunchingBomb();
        }

    }

    private void OnDisable()
    {
        StopAllCoroutines();
        InputMap.Overworld.ShootPrimary.performed -= TriggerPrimary;
        InputMap.Overworld.ShootSecondary.performed -= TriggerSecondary;
        InputMap.Disable();
    }

    public static bool IsMoving (out Vector2 direction)
    {
        
        direction = MovementInput;
        return direction != Vector2.zero;
    }

    public static bool IsBanking (out Vector3 direction)
    {
        direction = BankingInput; 
        return direction != Vector3.zero;
    }

    public static bool IsMovingVertically ( out Vector2 direction)
    {
        direction = VerticalMovement;
        return direction != Vector2.zero;
    }

    public static bool IsPitching ( out Vector2 direction)
    {
        direction = PitchingInput;
        return direction != Vector2.zero;
    }

    private void Paused(InputAction.CallbackContext context )
    {
        
        if (AlreadyOnMenu == false)
        {
            //UI_Manager.PausedByInput?.Invoke();
            AlreadyOnMenu = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0f;
            InputMap.Overworld.Disable();
            InputMap.Menu.Navigation.Enable();

        }
        else
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            if (PlayerIsAlive) { InputMap.Overworld.Enable(); }
            InputMap.Menu.Navigation.Disable();
            AlreadyOnMenu = !AlreadyOnMenu;
            //UI_Manager.UnPausedByInput?.Invoke();

        }
        
        OnPauseMenu();
        Debug.Log(AlreadyOnMenu);
    }

    void PlayerIsDead()
    {   
        PlayerIsAlive = false;
        InputMap.Overworld.Disable();
    }






}
