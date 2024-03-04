using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public delegate void Shoot();
    public static event Shoot ShootPrimary;
    public static event Shoot ShootSecondary;
    public static InputMap InputMap;
    public InputMap prova;

    public static InputManager Instance;

    public static Vector2 MovementInput => InputMap.Overworld.Movement.ReadValue<Vector2>();
    public static Vector3 BankingInput => InputMap.Overworld.Banking.ReadValue<Vector3>();

    public static Vector3 VerticalMovement => InputMap.Overworld.VerticalMovement.ReadValue<Vector2>();
    public static float ShootingInput => InputMap.Overworld.Shoot.ReadValue<float>();


    private void Awake()
    {
        InputMap = new InputMap();

    }
    private void OnEnable()
    {
        InputMap.Enable();
        
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

    void Shooting()
    {
        if (ShootingInput > 0) { ShootPrimary?.Invoke(); }
        else { ShootSecondary?.Invoke(); }
    }
        

}
