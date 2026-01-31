using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenu : MonoBehaviour
{
    public void GoToMenu()
    {
        Time.timeScale = 1f; // Unpause time
        PauseMenu.GameIsPaused = false; // Reset the static variable from your other script
        SceneManager.LoadScene("MainMenu"); // Change "MainMenu" to your real scene name
    }
}