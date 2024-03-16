using UnityEngine;
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
    [SerializeField] private AudioData AudioData;

    private float Volume = 0f;

    //Volume is saved in DB
    private void Awake()
    {
        Slider.maxValue = 1f;
        Slider.minValue = 0.0001f;
    }

    private void Start()
    {
        if (!PlayerPrefs.HasKey(AudioData.Type.ToString() + "Volume"))
        {
            Volume = 20 * Mathf.Log10(AudioData.DefaultVolume);
            PlayerPrefs.SetFloat(AudioData.Type.ToString() + "Volume", Volume);
        }
        else
        {
            Volume = PlayerPrefs.GetFloat(AudioData.Type.ToString() + "Volume");
        }

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

        AudioData.SubMixer.SetFloat(AudioData.Type.ToString()+"Volume", value);
        PlayerPrefs.SetFloat(AudioData.Type.ToString()+"Volume", value);
    }

}
