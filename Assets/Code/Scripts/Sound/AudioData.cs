using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class AudioData
{
    public enum AudioType
    {
        Master,
        Music,
        SFX
    }

    [SerializeField] public AudioMixer SubMixer;
    [SerializeField] public AudioType Type;
    [SerializeField][Range(0.0001f, 1f)] public float DefaultVolume = 1.0f;
}
