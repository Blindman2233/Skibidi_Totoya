using UnityEngine;

public class SimpleTeleporter : MonoBehaviour
{
    [Header("Option 1 (Press U)")]
    public Transform destination1;   // Where does U take you?
    public GameObject textOption1;   // The text object for U (e.g. "Town 2")

    [Header("Option 2 (Press Y)")]
    public Transform destination2;   // Where does Y take you?
    public GameObject textOption2;   // The text object for Y (e.g. "Town 3")

    [Header("Auto Teleport")]
    public bool autoTeleport = true;
    public Transform autoDestination;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip triggerClip;
    public float triggerVolume = 1f;
    public Vector2 randomPitch = new Vector2(1f, 1f);
    public bool playOnEnter = true;

    [Header("Exit Position")]
    public Vector2 exitOffset = new Vector2(1f, 0f);
    public bool useLocalExitOffset = true;
    public float postTeleportCooldown = 0.25f;
    static float globalNextTriggerAllowedTime;

    private GameObject player;       // To remember the player
    private bool isPlayerClose;

    void Start()
    {
        // Hide both text options when the game starts
        if (textOption1 != null) textOption1.SetActive(false);
        if (textOption2 != null) textOption2.SetActive(false);
        if (autoDestination == null) autoDestination = destination1 != null ? destination1 : destination2;
    }

    void Update()
    {
        if (isPlayerClose && player != null) { }
    }

    void Teleport(Transform target)
    {
        Vector3 offsetWorld = useLocalExitOffset
            ? target.TransformVector(new Vector3(exitOffset.x, exitOffset.y, 0f))
            : new Vector3(exitOffset.x, exitOffset.y, 0f);
        Vector3 newPos = target.position + offsetWorld;

        var rb = player.GetComponent<Rigidbody2D>();
        if (rb != null) rb.position = newPos;
        else player.transform.position = newPos;

        // Hide the texts immediately after teleporting
        if (textOption1 != null) textOption1.SetActive(false);
        if (textOption2 != null) textOption2.SetActive(false);
        isPlayerClose = false;
        globalNextTriggerAllowedTime = Time.time + postTeleportCooldown;
    }

    // --- TRIGGER DETECTION ---

    void OnTriggerEnter2D(Collider2D other)
    {
        if (Time.time < globalNextTriggerAllowedTime) return;
        if (other.CompareTag("Player"))
        {
            player = other.gameObject;
            isPlayerClose = true;

            if (playOnEnter && triggerClip != null)
            {
                if (audioSource != null)
                {
                    audioSource.pitch = Random.Range(randomPitch.x, randomPitch.y);
                    audioSource.PlayOneShot(triggerClip, triggerVolume);
                }
                else
                {
                    AudioSource.PlayClipAtPoint(triggerClip, transform.position, triggerVolume);
                }
            }

            if (autoTeleport && autoDestination != null)
            {
                Teleport(autoDestination);
            }
            else
            {
                if (textOption1 != null) textOption1.SetActive(true);
                if (textOption2 != null) textOption2.SetActive(true);
            }
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
