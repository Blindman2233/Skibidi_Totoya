using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private float moveInput;
    private Animator animator;

    // This variable holds the object we are currently touching
    private GameObject currentInteractable = null;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // --- 1. Movement Input ---
        moveInput = Input.GetAxisRaw("Horizontal");

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
        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            InteractableItem itemScript = currentInteractable.GetComponent<InteractableItem>();
            if (itemScript != null)
            {
                itemScript.Interact();
            }
        }
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
}