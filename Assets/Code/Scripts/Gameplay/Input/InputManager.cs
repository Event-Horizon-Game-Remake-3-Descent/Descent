using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public delegate void OnShoot();
    public static event OnShoot OnPrimaryCalled = ()=> { };
    public static event OnShoot OnSecondaryCalled = () => { };
    public delegate void OnPause();
    public static event OnPause OnPauseMenu = () => { };
    public static InputMap InputMap;

    bool AlreadyOnMenu;
    

    public static Vector2 MovementInput => InputMap.Overworld.Movement.ReadValue<Vector2>();
    public static Vector3 BankingInput => InputMap.Overworld.Banking.ReadValue<Vector3>();
    public static Vector2 VerticalMovement => InputMap.Overworld.VerticalMovement.ReadValue<Vector2>();

   


    private void Awake()
    {
        InputMap = new InputMap();
    }

    private void OnEnable()
    {
        InputMap.Enable();
        InputMap.Overworld.ShootPrimary.performed += TriggerPrimary;
        InputMap.Overworld.ShootSecondary.performed += TriggerSecondary;
        InputMap.Menu.Pause.performed += Paused;
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

    private void Paused(InputAction.CallbackContext context )
    {
        if (!AlreadyOnMenu)
        {
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0f;
            InputMap.Overworld.Disable();
            
            
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            InputMap.Overworld.Enable();
            

        }
        AlreadyOnMenu =! AlreadyOnMenu;
        OnPauseMenu();
    }






}
