using System;


using UnityEngine;
using UnityEngine.InputSystem;


public class GameManager : MonoBehaviour
{
    public static Action<float> GetMouseSens;
    
    public static float Score {  get; private set; }
    public static float MouseSens = 250f;

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

        if(!PlayerPrefs.HasKey("PlayerScore"))
        {
            PlayerPrefs.SetFloat("PlayerScore", 0);
        }
    }

    private void Start()
    {
        GetMouseSens += ChangeSensitivity;
        Score = 0f;
        
    }
    private void OnEnable()
    {

        ScoreCollectible.OnIncreaseScore += UpdateScore;
           
    }

    public void ChangeSensitivity(float value)
    {
        MouseSens = value;
        PlayerController.UpdatePlayerSens?.Invoke();
        PlayerPrefs.SetFloat("MouseSensitivity", MouseSens);
    }

    void UpdateScore(float value)
    {
        Score += value; 
        UI_Manager.UpdateUI?.Invoke();
        PlayerPrefs.SetFloat("PlayerScore", Score);

    }





    
    

    














}   



















