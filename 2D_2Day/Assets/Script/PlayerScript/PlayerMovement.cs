using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private float moveInput;

    // This variable holds the object we are currently touching
    private GameObject currentInteractable = null;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // --- 1. Movement Input ---
        moveInput = Input.GetAxisRaw("Horizontal");

        // --- 2. Interaction Input ---
        // If we press E AND we are currently touching an interactable object
        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            // Call the function on the object script
            InteractableItem itemScript = currentInteractable.GetComponent<InteractableItem>();
            if (itemScript != null)
            {
                itemScript.Interact();
            }
        }
    }

    void FixedUpdate()
    {
        // Move the player (Left/Right)
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    // --- TRIGGER DETECTION ---

    // When Player enters the object's area
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Interactable"))
        {
            currentInteractable = other.gameObject;
            Debug.Log("Can Interact! Press E"); // Helpful message in console
        }
    }

    // When Player leaves the object's area
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Interactable"))
        {
            if (other.gameObject == currentInteractable)
            {
                currentInteractable = null;
            }
        }
    }
}