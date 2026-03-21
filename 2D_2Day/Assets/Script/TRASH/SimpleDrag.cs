using UnityEngine;

public class SimpleDrag : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isDragging = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // When you click the mouse...
        if (Input.GetMouseButtonDown(0))
        {
            // Shoot a laser to see if we clicked THIS object or its children (walls)
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.transform.IsChildOf(transform))
            {
                isDragging = true;
            }
        }

        // When you let go...
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    void FixedUpdate()
    {
        if (isDragging)
        {
            // Move the Rigidbody to the mouse position
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            rb.MovePosition(new Vector2(mousePos.x, mousePos.y));
        }
    }
}