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
    [SerializeField][Range(0, 1f)] private float DefaultVolume = 1.0f;

    private float Volume = 0f;

    //Volume is saved in DB

    private void Awake()
    {
        if (!PlayerPrefs.HasKey(Type.ToString()+"Volume"))
        {
            Volume = 20 * Mathf.Log10(DefaultVolume);
            Debug.Log(Volume);
            PlayerPrefs.SetFloat(Type.ToString()+"Volume", Volume);
        }
        else
        {
            Volume = PlayerPrefs.GetFloat(Type.ToString()+"Volume");
        }
        Slider.maxValue = 1f;
        Slider.minValue = 0.0001f;
    }

    private void Start()
    {
        Slider.value = Mathf.Pow(10f, Volume / 20f);
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

        //from linear to Logarithmic
        value = 20 * Mathf.Log10(value);
        Volume = value;

        Debug.Log(Volume);

        SubMixer.audioMixer.SetFloat(Type.ToString()+"Volume", value);
        PlayerPrefs.SetFloat(Type.ToString()+"Volume", value);
    }

}
