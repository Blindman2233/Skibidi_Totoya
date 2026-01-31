using UnityEngine;
using UnityEngine.SceneManagement; // Needed to know which level we are in

public class PlayerSaveSystem : MonoBehaviour
{
    void Start()
    {
        // --- LOAD GAME LOGIC ---
        // 1. Check if we are supposed to be loading a saved game
        if (PlayerPrefs.GetInt("IsGameLoaded") == 1)
        {
            // 2. Read the saved position
            float x = PlayerPrefs.GetFloat("SavedX");
            float y = PlayerPrefs.GetFloat("SavedY");
            float z = PlayerPrefs.GetFloat("SavedZ");

            // 3. Teleport the player there
            transform.position = new Vector3(x, y, z);

            // 4. Important: Reset the flag so next time we play normally, we don't teleport
            PlayerPrefs.SetInt("IsGameLoaded", 0);

            Debug.Log("Game Loaded! Teleported to: " + transform.position);
        }
    }

    // Call this function to save progress
    public void SaveGame()
    {
        // 1. Save the Scene Name (e.g. "Town1")
        string currentScene = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("SavedLevel", currentScene);

        // 2. Save the Position (X, Y, Z)
        PlayerPrefs.SetFloat("SavedX", transform.position.x);
        PlayerPrefs.SetFloat("SavedY", transform.position.y);
        PlayerPrefs.SetFloat("SavedZ", transform.position.z);

        // 3. Mark that we have a valid save file
        PlayerPrefs.SetInt("HasSaveFile", 1);
        PlayerPrefs.Save(); // Force write to disk

        Debug.Log("Game Saved!");
    }

    // Optional: Press F5 to Quick Save (for testing)
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SaveGame();
        }
    }
}