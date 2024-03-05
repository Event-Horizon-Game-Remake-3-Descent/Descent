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

    //TO REMOVE
    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            if(!OnPause)
            {
                HUD.gameObject.SetActive(false);
                SettingsPanel.gameObject.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 0;
            }
            else
            {
                HUD.gameObject.SetActive(true);
                SettingsPanel.gameObject.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Time.timeScale = 1;
            }
            
            OnPause = !OnPause;
        }
    }
    ////////////////////////////////////////////

    public void QuitGame()
    {
        Application.Quit();
    }
}
