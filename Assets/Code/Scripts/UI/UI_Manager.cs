using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    [SerializeField] private RectTransform SettingsPanel;
    [SerializeField] private RectTransform HUD;

    bool OnPause = false;

    private void Awake()
    {
        HUD.gameObject.SetActive(true);
        SettingsPanel.gameObject.SetActive(false);
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
