using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractableItem : MonoBehaviour
{
    [Header("Scene Settings")]
    public string sceneToLoad;

    [Header("UI Settings")]
    [Tooltip("Drag the 'E' button object here")]
    public GameObject promptObject; // The floating "E" icon

    public void Interact()
    {
        if (sceneToLoad != "")
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.Log("Interacted with " + gameObject.name);
        }
    }

    // --- SHOW / HIDE PROMPT AUTOMATICALLY ---

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the thing colliding is the Player
        if (other.CompareTag("Player"))
        {
            if (promptObject != null)
            {
                promptObject.SetActive(true); // Show "E"
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
            }
        }
    }
}