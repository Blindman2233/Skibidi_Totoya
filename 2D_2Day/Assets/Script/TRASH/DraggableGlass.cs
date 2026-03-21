using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DraggableGlass : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isDragging = false;
    private Vector3 offset;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic; // Important: Stops gravity
    }

    void Update()
    {
        // 1. Detect Click
        if (Input.GetMouseButtonDown(0))
        {
            CheckForClick();
        }

        // 2. Stop Dragging
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    void FixedUpdate()
    {
        // 3. Move the Glass
        if (isDragging)
        {
            Vector3 mousePos = GetMouseWorldPos();
            rb.MovePosition(mousePos + offset);
        }
    }

    void CheckForClick()
    {
        Vector3 mousePos = GetMouseWorldPos();

        // Shoot a laser at the mouse position
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            // If we hit ANYTHING attached to this glass (Walls or Handle)
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                isDragging = true;
                offset = transform.position - mousePos;
            }
        }
    }

    Vector3 GetMouseWorldPos()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0; // Force Z to 0 so it stays on the 2D plane
        return pos;
    }
}