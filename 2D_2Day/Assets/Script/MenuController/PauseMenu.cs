using UnityEngine;
using UnityEngine.SceneManagement; // Needed for Menu/Quit
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject pauseMenuUI; // Drag your Panel object here
    public Slider volumeSlider;
    public Text volumeText; // Optional: Drag a Text element here to show %

    public static bool GameIsPaused = false; // Tracks if game is paused

    void Start()
    {
        // Initialize Volume Slider
        if (volumeSlider != null)
        {
            float savedVolume = PlayerPrefs.GetFloat("masterVolume", 1f);
            volumeSlider.value = savedVolume;
            AudioListener.volume = savedVolume;
            UpdateVolumeText(savedVolume);

            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    void Update()
    {
        // Check if player presses Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    // --- FUNCTION 1: RESUME GAME ---
    public void Resume()
    {
        pauseMenuUI.SetActive(false); // Hide the menu
        Time.timeScale = 1f;          // Unfreeze time
        GameIsPaused = false;
    }

    // --- FUNCTION 2: PAUSE GAME ---
    void Pause()
    {
        pauseMenuUI.SetActive(true);  // Show the menu
        Time.timeScale = 0f;          // Freeze time
        GameIsPaused = true;
    }

    // --- BUTTON FUNCTIONS ---

    public void LoadMenu()
    {
        Time.timeScale = 1f; // Always unfreeze before leaving the scene!
        SceneManager.LoadScene("MainMenu"); // Change to your menu scene name
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("masterVolume", volume);
        PlayerPrefs.Save();
        UpdateVolumeText(volume);
    }

    private void UpdateVolumeText(float volume)
    {
        if (volumeText != null)
        {
            volumeText.text = Mathf.RoundToInt(volume * 100f) + "%";
        }
    }
}