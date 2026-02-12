using UnityEngine;
using UnityEngine.UI;

public class VolumeSliderUI : MonoBehaviour
{
    public Slider slider;
    public Text valueText;

    void Awake()
    {
        if (slider == null) slider = GetComponentInChildren<Slider>(true);
        if (slider != null)
        {
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.wholeNumbers = false;
            slider.onValueChanged.AddListener(OnSliderChanged);
        }
    }

    void Start()
    {
        float v = PlayerPrefs.GetFloat("masterVolume", 1f);
        AudioListener.volume = v;
        if (slider != null) slider.value = v;
        UpdateText(v);
    }

    void OnDestroy()
    {
        if (slider != null) slider.onValueChanged.RemoveListener(OnSliderChanged);
    }

    void OnSliderChanged(float value)
    {
        float v = Mathf.Clamp01(value);
        AudioListener.volume = v;
        PlayerPrefs.SetFloat("masterVolume", v);
        UpdateText(v);
    }

    void UpdateText(float v)
    {
        if (valueText != null) valueText.text = Mathf.RoundToInt(v * 100f) + "%";
    }
}
