using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
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
    public static Action OnFlashingBlue;
    public static Action OnFlashingRed;
    // All panels and relatives TMPs
    [Header("References, don't touch it")]
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
    [SerializeField] private TMP_Text Score_text;
    [SerializeField] private TMP_Text Notification_text;
    [SerializeField] TMP_Text Lives_text;
    [SerializeField] TMP_Text SensValue_text;
    [SerializeField] TMP_Text Countdown_text;
    [SerializeField] Image OuterShield;
    [SerializeField] Image BlueKey;
    [SerializeField] Image YellowKey;
    [SerializeField] Image RedKey;
    // button references for gamepad navigation
    [SerializeField] GameObject AudioButton;
    [SerializeField] GameObject MasterSlider;
    [SerializeField] GameObject ControlsButton;
    [SerializeField] GameObject SensSlider;
    [Space]
    // Flashing Images
    [SerializeField] private CanvasGroup RedFlash;
    [SerializeField] private CanvasGroup BlueFlash;
    [Header("Flashing Screens Parameters")]
    [SerializeField] float FadeDuration;
    [SerializeField] float MaxAlpha;
    [Space]
    [Header("Time To Escape")]
    [SerializeField] float TimeForEscape;
    [SerializeField] float IncreaseTextSizeBy;
    [SerializeField] float StartingTextSize;




    PlayerController Player;

    bool OnPause = false;
    bool PlayerIsDead;

    private void Awake()
    { 
        Countdown_text.gameObject.SetActive(false);
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
        RedFlash.alpha = 0;
        BlueFlash.alpha = 0;
        BlueKey.color = Color.black;
        YellowKey.color = Color.black;
        RedKey.color = Color.black;
        StartingTextSize = Countdown_text.fontSize;
    }

  



    private void OnEnable()
    {
        InputManager.OnPauseMenu += PauseMenu;
        PlayerController.OnPlayerReady += (PlayerController playerController) => { Player = playerController; UpdateCollectibles(); HandleVisualShield(); };
        PlayerController.OnPlayerDead += () => PlayerIsDead = true;
        PlayerController.OnUpdatingUiCollect += UpdateCollectibles;
        PlayerController.OnUpdatingUiCollect += HandleVisualShield;
        PlayerController.OnPlayerDead += HideHUD;
        PlayerController.OnPlayerRespawned += ShowHUD;
        EscapeSequenceManager.OnEscapeSequenceTriggered += HideHUD;
        Boss.OnBossDefeat += StartCountDown;
        KeyCollectible.OnKeyCollected += EnableKey;
        InputManager.OnMinimapOpen += () => FullHUD.gameObject.SetActive(false);
        InputManager.OnMinimapClosed += () => FullHUD.gameObject.SetActive(true);
        Notify += Notifications;
        UpdateUI += UpdateCollectibles;
        PausedByInput +=()=> OnPause = true;
        UnPausedByInput +=()=> OnPause = false;
        OnFlashingBlue += FlashingBlue;
        OnFlashingRed += FlashingRed;
        
        //Collectible.OnIncreaseScore += (float value) => { UpdateCollectibles(); };
    }
       
        

    private void OnDisable()
    {
        InputManager.OnPauseMenu -= PauseMenu;
        PlayerController.OnUpdatingUiCollect -= UpdateCollectibles;
        PlayerController.OnUpdatingUiCollect -= HandleVisualShield;
        PlayerController.OnPlayerDead -= HideHUD;
        EscapeSequenceManager.OnEscapeSequenceTriggered -= HideHUD;
        Notify -= Notifications;
        UpdateUI -= UpdateCollectibles;
    }

    public void ShowSensitivityValue()
    {
        if (SensitivitySlider.isActiveAndEnabled) { GameManager.GetMouseSens(SensitivitySlider.value); }
        if (SensitivitySlider.isActiveAndEnabled) { SensValue_text.text = Mathf.RoundToInt( GameManager.MouseSens).ToString(); }
    }
        
   void FlashingRed()
    {
        StopCoroutine(FlashingScreen(BlueFlash));
        StartCoroutine(FlashingScreen(RedFlash));
    }

    void FlashingBlue()
    {
        StopCoroutine(FlashingScreen(RedFlash));
        StartCoroutine(FlashingScreen(BlueFlash));
    }

    void StartCountDown()
    {
        Countdown_text.gameObject.SetActive(true);
        StartCoroutine(Countdown());
    }

    private IEnumerator Countdown()
    {
        float currentTime = TimeForEscape;

        while (currentTime > 0)
        {
            Countdown_text.text = "" + currentTime;
            
            Countdown_text.fontSize += IncreaseTextSizeBy;
            yield return new WaitForSeconds(0.5f); 

            
            Countdown_text.fontSize = StartingTextSize;
            yield return new WaitForSeconds(0.5f); 

            currentTime--;
            
        }
        Countdown_text.text = "0";
        
    }

    IEnumerator FlashingScreen(CanvasGroup image)
    {
        float counter = 0f;
        while (counter < FadeDuration)
        {
            counter += Time.deltaTime;
            float alphaValue = Mathf.Lerp(0f, MaxAlpha, counter / FadeDuration);
            image.alpha = alphaValue;
            yield return null;
        }
        image.alpha = 1f;
        counter = 0f;
        while (counter < FadeDuration) 
        {
            counter += Time.deltaTime;
            float alphaValue = Mathf.Lerp(MaxAlpha, 0f, counter / FadeDuration);
            image.alpha = alphaValue;
            yield return null;
        }
        image.alpha = 0f;
    }

    void EnableKey(int value)
    {
        switch (value) 
        {
            case 0:
            {
                RedKey.color = Color.red;
                break;
            }
            case 1: 
            {
                YellowKey.color = Color.yellow; 
                break;
            }
            case 2:
            {
                BlueKey.color = Color.blue;
                break;
            }
        }
    }


    void UpdateCollectibles()
    {
        Shield_text.text = Mathf.Ceil(Player.HP).ToString();
        Score_text.text = "Score:" + MathF.Round( GameManager.Score).ToString();
        Lives_text.text = " x " + Player.Lives.ToString();
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

    public void PauseMenu()
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
            EventSystem.current.SetSelectedGameObject(AudioButton);
            
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
            EventSystem.current.SetSelectedGameObject(null);
            
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
        MainMenu.gameObject.SetActive(false);
        BackButton.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(SensSlider);
    }

    public void OpenAudioSettings()
    {
        MainMenu.gameObject.SetActive(false);
        AudioMenu.gameObject.SetActive(true);
        BackButton.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(MasterSlider);
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
        EventSystem.current.SetSelectedGameObject(AudioButton);
    }

    public void Resume()
    {
        SettingsPanel.gameObject.SetActive(false);
        QuitButton.gameObject.SetActive(false);
        ResumeButton.gameObject.SetActive(false);
        BackButton.gameObject.SetActive(false);
        if(!PlayerIsDead) { InputManager.InputMap.Overworld.Enable(); }
        OnPause = false;
        InputManager.Resume?.Invoke();
        Time.timeScale = 1.0f;
        EventSystem.current.SetSelectedGameObject(null);
    }

    void HideHUD()
    {
        FullHUD.anchoredPosition = Vector3.up * 1500;
    }

    void ShowHUD()
    {
        FullHUD.anchoredPosition = Vector3.zero;
    }
        
        
    public void QuitGame()
    {
        Application.Quit();
    }
    
}
