using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{

    [Header("Audio Settings")]
    [SerializeField] private List<AudioData> AudioDatas;
    [SerializeField] private AudioClip MainMusic;
    [SerializeField] private AudioClip BossMusic;
    [SerializeField] private AudioClip EscapeMusic;
    private AudioSource BackGroundMusic;

    private void Awake()
    {
        for(int i = 0; i < AudioDatas.Count; i++)
            AudioDatas[i].SubMixer.SetFloat(AudioDatas[i].Type.ToString() + "Volume", -80f);

        BackGroundMusic = GetComponent<AudioSource>();
        BackGroundMusic.clip = MainMusic;
        BackGroundMusic.Play();
    }

    private void Start()
    {
        float volume = 0f;
        for (int i = 0; i < AudioDatas.Count; i++)
        {
            if (!PlayerPrefs.HasKey(AudioDatas[i].Type.ToString() + "Volume"))
            {
                volume = 20 * Mathf.Log10(AudioDatas[i].DefaultVolume);
                PlayerPrefs.SetFloat(AudioDatas[i].Type.ToString() + "Volume", volume);
            }
            else
            {
                volume = PlayerPrefs.GetFloat(AudioDatas[i].Type.ToString() + "Volume");
            }
            
            AudioDatas[i].SubMixer.SetFloat(AudioDatas[i].Type.ToString() + "Volume", volume);
        }
    }

    private void EnableBossMusic()
    {
        BackGroundMusic.Pause();
        BackGroundMusic.clip = BossMusic;
        BackGroundMusic.Play();
    }

    private void EnableEscapeMusic()
    {
        BackGroundMusic.Pause();
        BackGroundMusic.clip = EscapeMusic;
        BackGroundMusic.Play();
    }

    private void OnEnable()
    {
        Boss.OnBossTrigger += EnableBossMusic;
        Boss.OnBossDefeat += EnableEscapeMusic;
    }
}
