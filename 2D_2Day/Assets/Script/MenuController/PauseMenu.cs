using UnityEngine;
using UnityEngine.SceneManagement; // Needed for Menu/Quit

public class PauseMenu : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject pauseMenuUI; // Drag your Panel object here

    public static bool GameIsPaused = false; // Tracks if game is paused

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
}