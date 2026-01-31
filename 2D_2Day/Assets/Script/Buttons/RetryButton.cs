using UnityEngine;
using UnityEngine.SceneManagement; // Needed to load scenes

public class RetryButton : MonoBehaviour
{
    // Connect this function to your Button's "On Click" event
    public void RestartLevel()
    {
        // 1. Unpause the game (Critical!)
        Time.timeScale = 1f;

        // 2. Get the name of the current scene and reload it
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}