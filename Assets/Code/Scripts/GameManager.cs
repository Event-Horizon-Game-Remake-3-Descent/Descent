using System;


using UnityEngine;
using UnityEngine.InputSystem;


public class GameManager : MonoBehaviour
{
    public static Action<float> GetMouseSens;
    
    public static float Score {  get; private set; }
    public static float MouseSens;
    


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
        GetMouseSens += ChangeSensitivity;
        
    }
    private void OnEnable()
    {
        
        ScoreCollectible.OnIncreaseScore += (float value) => { Score += value; UI_Manager.UpdateUI?.Invoke(); };
           
    }

    public void ChangeSensitivity(float value)
    {
        MouseSens = value;
        PlayerController.UpdatePlayerSens?.Invoke();
        PlayerPrefs.SetFloat("MouseSensitivity", MouseSens);
    }





    
    

    














}   



















