using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public static Action<string> Notify;
    public static Action UpdateUI;
    public static Action PausedByInput;
    public static Action UnPausedByInput;
    [SerializeField] private RectTransform SettingsPanel;
    [SerializeField] private RectTransform HUD;
    [SerializeField] private RectTransform FullHUD;
    [SerializeField] private RectTransform MainMenu;
    [SerializeField] private RectTransform ControlsMenu;
    [SerializeField] private RectTransform AudioMenu;
    [SerializeField] private RectTransform QuitButton;
    [SerializeField] private RectTransform ResumeButton;
    [SerializeField] private RectTransform BackButton;
    [SerializeField] private Slider SensitivitySlider;
    [SerializeField] private TMP_Text Shield_text;
    [SerializeField] private Image OuterShield;
    [SerializeField] private TMP_Text Score_text;
    [SerializeField] private TMP_Text Notification_text;
    [SerializeField] TMP_Text SensValue_text;
    

    PlayerController Player;

    bool OnPause = false;

    private void Awake()
    {
        HUD.gameObject.SetActive(true);
        SettingsPanel.gameObject.SetActive(false);
        MainMenu.gameObject.SetActive(false);
        ControlsMenu.gameObject.SetActive(false);
        QuitButton.gameObject.SetActive(false);
        ResumeButton.gameObject.SetActive(false);
        BackButton.gameObject.SetActive(false);
        SensValue_text.text = Mathf.RoundToInt(GameManager.MouseSens).ToString(); 
    }

    private void Start()
    {
        SensValue_text.text = Mathf.RoundToInt (PlayerPrefs.GetFloat("MouseSensitivity")).ToString();
        InputManager.InputMap.Overworld.SwitchCamera.started +=(InputAction.CallbackContext hud)=> FullHUD.gameObject.SetActive(false);
        InputManager.InputMap.Overworld.SwitchCamera.canceled += (InputAction.CallbackContext hud) => FullHUD.gameObject.SetActive(true);
        
    }

    

    private void OnEnable()
    {
        InputManager.OnPauseMenu += Menu;
        PlayerController.OnPlayerReady += (PlayerController playerController) => { Player = playerController; UpdateCollectibles(); HandleVisualShield(); };
        PlayerController.OnUpdatingUiCollect += UpdateCollectibles;
        PlayerController.OnUpdatingUiCollect += HandleVisualShield;
        Notify += Notifications;
        UpdateUI += UpdateCollectibles;
        PausedByInput +=()=> OnPause = true;
        UnPausedByInput +=()=> OnPause = false;
        
        //Collectible.OnIncreaseScore += (float value) => { UpdateCollectibles(); };
    }
       
        

    private void OnDisable()
    {
        InputManager.OnPauseMenu -= Menu;
        PlayerController.OnUpdatingUiCollect -= UpdateCollectibles;
        PlayerController.OnUpdatingUiCollect -= HandleVisualShield;
        Notify -= Notifications;
        UpdateUI -= UpdateCollectibles;
    }

    public void ShowSensitivityValue()
    {
        if(SensitivitySlider.isActiveAndEnabled) { SensValue_text.text = Mathf.RoundToInt( GameManager.MouseSens).ToString(); }
    }
        

   

    void UpdateCollectibles()
    {
        Shield_text.text = Player.HP.ToString();
        Score_text.text = "Score:" + MathF.Round( GameManager.Score).ToString();
    }


    void Notifications(string text)
    {
        Notification_text.text = text;
        StartCoroutine(NotificationTimer());
        
    }

    IEnumerator NotificationTimer()
    {
        yield return new WaitForSecondsRealtime(1);
        Notification_text.text = " ";
        yield return null;
    }

    public void Menu()
    {
        if (OnPause == false)
        {
            OnPause = true;
            InputManager.IsPaused?.Invoke();
            SettingsPanel.gameObject.SetActive(true);
            MainMenu.gameObject.SetActive(true);
            QuitButton.gameObject.SetActive(true);
            ResumeButton.gameObject.SetActive(true);
            AudioMenu.gameObject.SetActive(false);
            ControlsMenu.gameObject.SetActive(false);
            AudioMenu.gameObject.SetActive(false);
            
        }
        else
        {
            SettingsPanel.gameObject.SetActive(false);
            MainMenu.gameObject.SetActive(false);
            QuitButton.gameObject.SetActive(false);
            ResumeButton.gameObject.SetActive(false);
            BackButton.gameObject.SetActive(false);
            OnPause = false;
            InputManager.Resume?.Invoke();
            
        }
    }

    void HandleVisualShield()
    {
        if (Player.HP <= 100)
        {
            OuterShield.fillAmount = Player.HP / 100;
        }
        else if (Player.HP >= 100) 
        {
            OuterShield.fillAmount = 1;
        }
    }

    public void OpenControlsMenu()
    {
        ControlsMenu.gameObject.SetActive(true);
        MainMenu.gameObject.SetActive(false) ;
        BackButton.gameObject.SetActive(true) ;
    }

    public void OpenAudioSettings()
    {
        MainMenu.gameObject.SetActive(false);
        AudioMenu.gameObject.SetActive(true);
        BackButton.gameObject.SetActive(true);
    }

    public void BackToMainMenu()
    {
        SettingsPanel.gameObject.SetActive(true);
        MainMenu.gameObject.SetActive(true);
        QuitButton.gameObject.SetActive(true);
        ResumeButton.gameObject.SetActive(true);
        AudioMenu.gameObject.SetActive(false);
        ControlsMenu.gameObject .SetActive(false);  
        BackButton.gameObject.SetActive(false);
    }

    public void Resume()
    {
        SettingsPanel.gameObject.SetActive(false);
        QuitButton.gameObject.SetActive(false);
        ResumeButton.gameObject.SetActive(false);
        BackButton.gameObject.SetActive(false);
        InputManager.InputMap.Overworld.Enable();
        OnPause = false;
        InputManager.Resume?.Invoke();
        Time.timeScale = 1.0f;
        
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    
}
