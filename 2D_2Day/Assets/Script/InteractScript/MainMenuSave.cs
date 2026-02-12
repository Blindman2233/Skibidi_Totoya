using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Needed if you want to disable the button

public class MainMenuSave : MonoBehaviour
{
    public Button continueButton; // Drag your Continue button here

    void Start()
    {
        // Optional: If there is no save file, gray out the button
        if (continueButton != null)
        {
            if (PlayerPrefs.GetInt("HasSaveFile") == 0)
            {
                continueButton.interactable = false; // Disable button
            }
        }
    }

    public void LoadSavedGame()
    {
        // 1. Check if we actually have a save
        if (PlayerPrefs.GetInt("HasSaveFile") == 1)
        {
            // 2. Tell the game "We are loading"
            PlayerPrefs.SetInt("IsGameLoaded", 1);

            // 3. Get the level name we saved inside "PlayerSaveSystem"
            string levelToLoad = PlayerPrefs.GetString("SavedLevel");

            // 4. Load that scene
            SceneManager.LoadScene(levelToLoad);
        }
    }

    // Connect this to your "New Game" button to clear old saves
    public void NewGame()
    {
        // Reset the loading flag so we start at the normal spawn point
        PlayerPrefs.SetInt("IsGameLoaded", 0);

        // Load Level 1 (Replace with your first level name)
        SceneManager.LoadScene("Chapter01");
    }
}