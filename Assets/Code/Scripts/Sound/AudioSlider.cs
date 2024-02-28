using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSlider : MonoBehaviour
{
    enum AudioType
    {
        Master,
        Music,
        SFX
    }

    [SerializeField] private Slider Slider;
    [SerializeField] private AudioMixerGroup SubMixer;
    [SerializeField] private AudioType Type;

    private float Volume = 0f;
    private float DefaultVolume = 0f;

    private void Awake()
    {
        if (!PlayerPrefs.HasKey(Type.ToString()+"Volume"))
        {
            Volume = DefaultVolume;
            PlayerPrefs.SetFloat(Type.ToString()+"Volume", Volume);
        }
        else
        {
            Volume = PlayerPrefs.GetFloat(Type.ToString()+"Volume");
        }
        Slider.maxValue = 0f;
        Slider.minValue = -80f;
    }

    private void Start()
    {
        Slider.value = Volume;
    }

    private void OnEnable()
    {
        Slider.onValueChanged.AddListener(ChangeValue);
    }

    private void OnDisable()
    {
        Slider.onValueChanged.RemoveListener(ChangeValue);
    }

    private void ChangeValue(float value)
    {
        if (Slider.value != Volume)
            Volume = Slider.value;

        SubMixer.audioMixer.SetFloat(Type.ToString()+"Volume", value);
        PlayerPrefs.SetFloat(Type.ToString()+"Volume", value);
    }

}
