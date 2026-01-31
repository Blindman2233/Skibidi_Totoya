using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool hasSaved = false; // Ensure we only save once per checkpoint

    void OnTriggerEnter2D(Collider2D other)
    {
        // If the player touches this checkpoint...
        if (other.CompareTag("Player") && !hasSaved)
        {
            // ...Find the Save Script on the player and run it
            PlayerSaveSystem saveScript = other.GetComponent<PlayerSaveSystem>();
            if (saveScript != null)
            {
                saveScript.SaveGame();
                hasSaved = true; // Don't save again if they walk back and forth
                Debug.Log("Checkpoint Reached! Game Saved.");
            }
        }
    }
}