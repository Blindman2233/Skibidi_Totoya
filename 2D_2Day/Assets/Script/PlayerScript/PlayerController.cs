using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private float moveInput;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (DialogueManager.Instance != null && DialogueManager.Instance.isDialogueActive)
        {
            moveInput = 0f;
            animator.SetFloat("Speed", 0f);
            return;
        }

        moveInput = Input.GetAxisRaw("Horizontal");

        animator.SetFloat("Speed", Mathf.Abs(moveInput));

        if (moveInput > 0) // Moving Right
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (moveInput < 0) // Moving Left
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    void FixedUpdate()
    {
        // ---  Movement ---
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

}
