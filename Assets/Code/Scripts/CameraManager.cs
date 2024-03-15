using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{

    [SerializeField] Camera PlayerCamera;
    [SerializeField] Camera RearCamera;
    [SerializeField] Camera DeathCamera;
    public bool RearCamActive;
   
    void Start()
    {
        RearCamera.enabled = false;
        DeathCamera.enabled = false;
        InputManager.InputMap.Overworld.SwitchCamera.started += SwitchCam;
        InputManager.InputMap.Overworld.SwitchCamera.canceled += SwitchCam;
    }

    private void OnEnable()
    {
        PlayerController.OnPlayerDead += DeathCam;
    }
    
   

    void SwitchCam(InputAction.CallbackContext context)
    {
        if (!RearCamActive)
        {
            PlayerCamera.enabled = false;
            RearCamera.enabled = true;
        }
        else
        {
            PlayerCamera.enabled = true;
            RearCamera.enabled = false;
        }
        RearCamActive = !RearCamActive;

    }

    void DeathCam()
    {
        PlayerCamera.enabled = false;
        RearCamera.enabled = false;
        DeathCamera.enabled = true;
    }
}
