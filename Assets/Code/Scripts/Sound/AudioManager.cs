using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] List<AudioData> AudioDatas;

    private void Awake()
    {
        for(int i = 0; i < AudioDatas.Count; i++)
            AudioDatas[i].SubMixer.SetFloat(AudioDatas[i].Type.ToString() + "Volume", -80f);
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
}
