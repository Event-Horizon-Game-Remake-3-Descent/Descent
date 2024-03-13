using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] Camera PlayerCamera;
    [SerializeField] Camera RearCamera;
    [SerializeField] Camera DeathCamera;
    [SerializeField] Slider SensitivitySlider;
    public static float Score {  get; private set; }
    public static float MouseSens = 250;
    public bool RearCamActive;


    private void Awake()
    {
        if (PlayerPrefs.HasKey("MouseSensitivity"))
        {
            MouseSens = PlayerPrefs.GetFloat("MouseSensitivity");
        }
        else
        {
            PlayerPrefs.SetFloat("MouseSensitivity", MouseSens);
        }
    }

    private void Start()
    {
        SensitivitySlider.value = MouseSens;
        MouseSens = SensitivitySlider.value;
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

    public void ChaangeSensitivity()
    {
        MouseSens = SensitivitySlider.value;
        PlayerPrefs.SetFloat("MouseSensitivity", MouseSens);
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



















