using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] Camera PlayerCamera;
    [SerializeField] Camera RearCamera;
    [SerializeField] Camera DeathCamera;
    public static float Score {  get; private set; }
    public bool RearCamActive;


    private void Start()
    {
        RearCamera.enabled = false;
        DeathCamera.enabled = false;
        InputManager.InputMap.Overworld.SwitchCamera.started += SwitchCam;
        InputManager.InputMap.Overworld.SwitchCamera.canceled += SwitchCam;
    }
    private void OnEnable()
    {
        PlayerController.OnPlayerDead += DeathCam;
        Collectible.OnIncreaseScore += (float value) => { Score += value; UI_Manager.UpdateUI?.Invoke(); };
        
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
            RearCamera.enabled=false;
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



















