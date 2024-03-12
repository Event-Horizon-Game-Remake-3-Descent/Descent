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
    [SerializeField] private RectTransform SettingsPanel;
    [SerializeField] private RectTransform HUD;
    [SerializeField] private RectTransform FullHUD;
    [SerializeField] private TMP_Text Shield_text;
    [SerializeField] private Image OuterShield;
    [SerializeField] private TMP_Text Score_text;
    [SerializeField] private TMP_Text Notification_text;

    PlayerController Player;

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
        PlayerController.OnPlayerReady += (PlayerController playerController) => { Player = playerController; UpdateCollectibles(); HandleVisualShield(); };
        PlayerController.OnUpdatingUiCollect += UpdateCollectibles;
        PlayerController.OnUpdatingUiCollect += HandleVisualShield;
        Notify += Notifications;
        UpdateUI += UpdateCollectibles;
        //Collectible.OnIncreaseScore += (float value) => { UpdateCollectibles(); };
    }
       
        

    private void OnDisable()
    {
        InputManager.OnPauseMenu -= Menu;
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

    public void QuitGame()
    {
        Application.Quit();
    }
    
}
