using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class IntroInputManager : MonoBehaviour
{
    public static InputMap InputMap;

    public delegate void OnIntro();
    public static event OnIntro OnSkip;
    private void Awake()
    {
        InputMap = new InputMap();
    }
    private void OnEnable()
    {
        InputMap.Disable();
        InputMap.IntroScene.Enable();
        InputMap.IntroScene.Skip.performed += Skip;
    }

    private void OnDisable()
    {
        InputMap.IntroScene.Skip.performed -= Skip;
        InputMap.Disable();
    }

    void Skip(InputAction.CallbackContext context)
    {
        OnSkip();
    }

}
