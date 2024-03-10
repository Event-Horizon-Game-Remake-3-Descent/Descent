using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    [SerializeField] private RectTransform SettingsPanel;
    [SerializeField] private RectTransform HUD;
    [SerializeField] private RectTransform FullHUD;




    bool OnPause = false;

    private void Awake()
    {
        HUD.gameObject.SetActive(true);
        SettingsPanel.gameObject.SetActive(false);
    }

    private void Start()
    {
        InputManager.InputMap.Overworld.SwitchCamera.started +=(InputAction.CallbackContext hud)=> FullHUD.gameObject.SetActive(false);
        InputManager.InputMap.Overworld.SwitchCamera.canceled += (InputAction.CallbackContext hud) => FullHUD.gameObject.SetActive(true);

    }

    

    private void OnEnable()
    {
        InputManager.OnPauseMenu += Menu;
        
    }

    private void OnDisable()
    {
        InputManager.OnPauseMenu -= Menu;
    }

   

    
    private void Menu()
    {
        if(!OnPause)
        {
            
            SettingsPanel.gameObject.SetActive(true);
        }
        else
        {
            SettingsPanel.gameObject.SetActive(false);
        }
        OnPause = !OnPause;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
}
