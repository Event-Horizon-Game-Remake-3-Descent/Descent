using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    [SerializeField] private RectTransform SettingsPanel;
    [SerializeField] private RectTransform HUD;
    

    float EnergyLeft;

    bool OnPause = false;

    private void Awake()
    {
        HUD.gameObject.SetActive(true);
        SettingsPanel.gameObject.SetActive(false);
    }

    private void Start()
    {
       
        
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
