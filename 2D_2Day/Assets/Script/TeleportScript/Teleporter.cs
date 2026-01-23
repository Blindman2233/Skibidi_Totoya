using UnityEngine;

public class SimpleTeleporter : MonoBehaviour
{
    [Header("Option 1 (Press U)")]
    public Transform destination1;   // Where does U take you?
    public GameObject textOption1;   // The text object for U (e.g. "Town 2")

    [Header("Option 2 (Press Y)")]
    public Transform destination2;   // Where does Y take you?
    public GameObject textOption2;   // The text object for Y (e.g. "Town 3")

    private GameObject player;       // To remember the player
    private bool isPlayerClose;

    void Start()
    {
        // Hide both text options when the game starts
        if (textOption1 != null) textOption1.SetActive(false);
        if (textOption2 != null) textOption2.SetActive(false);
    }

    void Update()
    {
        // Only listen for keys if the player is standing here
        if (isPlayerClose && player != null)
        {
            // Press U -> Go to Destination 1
            if (Input.GetKeyDown(KeyCode.U) && destination1 != null)
            {
                Teleport(destination1);
            }

            // Press Y -> Go to Destination 2
            if (Input.GetKeyDown(KeyCode.Y) && destination2 != null)
            {
                Teleport(destination2);
            }
        }
    }

    void Teleport(Transform target)
    {
        // Move the player instantly
        player.transform.position = target.position;

        // Hide the texts immediately after teleporting
        if (textOption1 != null) textOption1.SetActive(false);
        if (textOption2 != null) textOption2.SetActive(false);
        isPlayerClose = false;
    }

    // --- TRIGGER DETECTION ---

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject;
            isPlayerClose = true;

            // Show both text options
            if (textOption1 != null) textOption1.SetActive(true);
            if (textOption2 != null) textOption2.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
            isPlayerClose = false;

            // Hide both text options
            if (textOption1 != null) textOption1.SetActive(false);
            if (textOption2 != null) textOption2.SetActive(false);
        }
    }
}