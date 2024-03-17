using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class RumbleManager : MonoBehaviour
{
    public static RumbleManager Instance;
    
    private Gamepad CurrentGamepad;
    private float Attenuation = 1f;

    public void SetRumbleAttenuator(float attenuation)
    {
        Attenuation = attenuation;
    }

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        CurrentGamepad = Gamepad.current;
    }

    public void AddRumble(float lowFrequency, float highFrequency, float duration)
    {
        StartCoroutine(Rumble(lowFrequency * Attenuation, highFrequency * Attenuation, duration));
    }

    private IEnumerator Rumble(float lowFrequency, float highFrequency, float duration)
    {
        if (CurrentGamepad == null)
            yield break;

        CurrentGamepad.SetMotorSpeeds(lowFrequency, highFrequency);
        yield return new WaitForSeconds(duration);
        CurrentGamepad.SetMotorSpeeds(0f, 0f);
    }
}
