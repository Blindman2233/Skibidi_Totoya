using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip[] footstepClips;
    [Tooltip("Time between footsteps in seconds")]
    public float footstepInterval = 0.5f;
    [Tooltip("Minimum pitch for randomization")]
    public float minPitch = 0.9f;
    [Tooltip("Maximum pitch for randomization")]
    public float maxPitch = 1.1f;

    private float footstepTimer;

    private Rigidbody2D rb;
    private float moveInput;
    private Animator animator;

    // This variable holds the object we are currently touching
    private GameObject currentInteractable = null;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        // --- 1. Movement Input ---
        moveInput = Input.GetAxisRaw("Horizontal");

        // --- Audio Logic ---
        // Only play if trying to move AND actually moving (prevents sound when pushing walls)
        if (Mathf.Abs(moveInput) > 0.1f && Mathf.Abs(rb.linearVelocity.x) > 0.1f)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0)
            {
                PlayFootstep();
                footstepTimer = footstepInterval;
            }
        }
        else
        {
            footstepTimer = 0;
        }

        // --- 2. Animation Logic ---
        // Send positive speed to the Animator (0 = Idle, >0 = Run)
        animator.SetFloat("Speed", Mathf.Abs(moveInput));

        // --- 3. Flip the Character (Safely) ---
        // We use Mathf.Abs to keep your current size (0.64) and just change the sign (+/-)

        if (moveInput > 0) // Moving Right
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (moveInput < 0) // Moving Left
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        // --- 4. Interaction Input ---
        /*if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            InteractableItem itemScript = currentInteractable.GetComponent<InteractableItem>();
            if (itemScript != null)
            {
                itemScript.Interact();
            }
        }*/
    }

    void FixedUpdate()
    {
        // Move the player
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    // --- TRIGGER DETECTION ---
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Interactable"))
        {
            currentInteractable = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Interactable") && other.gameObject == currentInteractable)
        {
            currentInteractable = null;
        }
    }

    private void PlayFootstep()
    {
        if (footstepClips != null && footstepClips.Length > 0 && audioSource != null)
        {
            audioSource.pitch = Random.Range(minPitch, maxPitch);
            audioSource.PlayOneShot(footstepClips[Random.Range(0, footstepClips.Length)]);
        }
    }
}