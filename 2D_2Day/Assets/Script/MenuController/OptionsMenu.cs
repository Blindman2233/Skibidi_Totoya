using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Slider masterVolumeSlider;
    public AudioSource soundtrackSource;
    public AudioClip soundtrackClip;
    public Text volumeText;

    void Start()
    {
        float v = PlayerPrefs.GetFloat("masterVolume", 1f);
        AudioListener.volume = v;
        if (masterVolumeSlider == null)
        {
            masterVolumeSlider = GetComponentInChildren<Slider>(true);
        }
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.minValue = 0f;
            masterVolumeSlider.maxValue = 1f;
            masterVolumeSlider.wholeNumbers = false;
            masterVolumeSlider.value = v;
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        }
        UpdateVolumeText(v);
        ApplySoundtrack();
    }

    void SetMasterVolume(float value)
    {
        AudioListener.volume = Mathf.Clamp01(value);
        if (soundtrackSource != null) soundtrackSource.volume = AudioListener.volume;
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        UpdateVolumeText(AudioListener.volume);
    }

    public void ApplySoundtrack()
    {
        if (soundtrackSource == null) return;
        if (soundtrackClip != null)
        {
            if (soundtrackSource.clip != soundtrackClip) soundtrackSource.clip = soundtrackClip;
            if (!soundtrackSource.isPlaying) soundtrackSource.Play();
        }
    }

    public void StopSoundtrack()
    {
        if (soundtrackSource != null && soundtrackSource.isPlaying) soundtrackSource.Stop();
    }

    public void SetSoundtrackClip(AudioClip clip)
    {
        soundtrackClip = clip;
        if (soundtrackSource != null) soundtrackSource.clip = soundtrackClip;
    }

    void UpdateVolumeText(float v)
    {
        if (volumeText != null) volumeText.text = Mathf.RoundToInt(v * 100f) + "%";
    }
}
