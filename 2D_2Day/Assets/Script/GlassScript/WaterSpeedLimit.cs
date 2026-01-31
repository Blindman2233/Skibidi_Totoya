using UnityEngine;

public class WaterSpeedLimit : MonoBehaviour
{
    public float maxSpeed = 10f; // Try 10 or 15
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // If water is falling faster than the limit...
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            // ...clamp it to the max speed
            rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, maxSpeed);
        }
    }
}