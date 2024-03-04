using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public delegate void OnShoot();
    public static event OnShoot OnPrimaryCalled;
    public static event OnShoot OnSecondaryCalled;
    public static InputMap InputMap;
    public InputMap prova;

    public static InputManager Instance;

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
        InputMap.Overworld.ShootPrimary.performed += (InputAction.CallbackContext f) => OnPrimaryCalled();
        InputMap.Overworld.ShootSecondary.performed += (InputAction.CallbackContext f) => OnSecondaryCalled();
    }

    private void OnDisable()
    {
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

    
    
}
