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
    
    [SerializeField] FreeLookCamera MapCamera;
    public bool RearCamActive;
   
    void Start()
    {
        RearCamera.enabled = false;
        DeathCamera.enabled = false;
        
        MapCamera.gameObject.SetActive(false);
        InputManager.InputMap.Overworld.SwitchCamera.started += SwitchCam;
        InputManager.InputMap.Overworld.SwitchCamera.canceled += SwitchCam;
    }

    private void OnEnable()
    {
        EscapeSequenceManager.OnEscapeSequenceTriggered += EscapeCam;
        PlayerController.OnPlayerDead += DeathCam;
        PlayerController.OnPlayerRespawned += CameraAfterRespawn;
        InputManager.OnMinimapOpen += MiniMapOpen;
        InputManager.OnMinimapClosed += MiniMapClosed;
    }

    private void OnDisable()
    {
        PlayerController.OnPlayerDead -= DeathCam;
        PlayerController.OnPlayerRespawned -= CameraAfterRespawn;
        InputManager.OnMinimapOpen -= MiniMapOpen;
        InputManager.OnMinimapClosed -= MiniMapClosed;
        InputManager.InputMap.Overworld.SwitchCamera.started -= SwitchCam;
        InputManager.InputMap.Overworld.SwitchCamera.canceled -= SwitchCam;
        EscapeSequenceManager.OnEscapeSequenceTriggered -= EscapeCam;
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

    void CameraAfterRespawn()
    {
        PlayerCamera.enabled = true;
        RearCamera.enabled = false;
        DeathCamera.enabled = false;
    }

    void EscapeCam()
    {
        PlayerCamera.enabled = false;
        RearCamera.enabled = false;
        DeathCamera.enabled = false;
        
    }

    void MiniMapOpen()
    {
        PlayerCamera.enabled = false;
        RearCamera.enabled = false;
        DeathCamera.enabled = false;
        MapCamera.gameObject.SetActive(true);
    }

    void MiniMapClosed()
    {
        PlayerCamera.enabled = true;
        RearCamera.enabled = false;
        DeathCamera.enabled = false;
        MapCamera.gameObject.SetActive(false);
    }
}
