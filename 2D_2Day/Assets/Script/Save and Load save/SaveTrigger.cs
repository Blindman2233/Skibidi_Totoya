using UnityEngine;

/// <summary>
/// Attach to an in-game object (e.g. a save point). When player interacts (e.g. press E or OnTriggerEnter),
/// open the Save screen UI. Assign saveLoadUIPanel and set isSaveMode true.
/// </summary>
public class SaveTrigger : MonoBehaviour
{
    [Header("Save UI")]
    public GameObject saveLoadUIPanel;

    [Header("Interaction")]
    [Tooltip("Key to open save menu (optional; leave empty if using trigger only).")]
    public KeyCode openKey = KeyCode.None;
    [Tooltip("If true, open save menu on trigger enter (e.g. player enters collider).")]
    public bool openOnTriggerEnter;
    [Tooltip("Tag of object that can trigger (e.g. Player).")]
    public string triggerTag = "Player";

    private bool playerInRange;

    private void Update()
    {
        if (openKey != KeyCode.None && Input.GetKeyDown(openKey) && playerInRange)
            OpenSaveMenu();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (openOnTriggerEnter && other.CompareTag(triggerTag))
        {
            playerInRange = true;
            OpenSaveMenu();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag(triggerTag))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(triggerTag))
            playerInRange = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (openOnTriggerEnter && other.CompareTag(triggerTag))
        {
            playerInRange = true;
            OpenSaveMenu();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(triggerTag))
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(triggerTag))
            playerInRange = false;
    }

    /// <summary>
    /// Call from button or other script to open Save screen.
    /// </summary>
    public void OpenSaveMenu()
    {
        if (saveLoadUIPanel == null) return;
        var ui = saveLoadUIPanel.GetComponent<SaveLoadUI>();
        if (ui != null)
            ui.SetSaveMode(true);
        saveLoadUIPanel.SetActive(true);
    }
}
