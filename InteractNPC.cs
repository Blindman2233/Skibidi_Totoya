using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractNPC : MonoBehaviour
{
    [Header("Scene Settings")]
    public GameObject text;

    [Header("UI Settings")]
    [Tooltip("Drag the 'E' button object here")]
    public GameObject promptObject; // The floating "E" icon

    // --- SHOW / HIDE PROMPT AUTOMATICALLY ---

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the thing colliding is the Player
        if (other.CompareTag("Player"))
        {
            if (promptObject != null)
            {
                promptObject.SetActive(true);
                text.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (promptObject != null)
            {
                promptObject.SetActive(false); // Hide "E"
                text.SetActive(false);
            }
        }
    }
}