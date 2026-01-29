using UnityEngine;

public class MouseFollower : MonoBehaviour
{
    [Header("Setup")]
    public Transform player;
    public Camera mainCam;

    [Header("Settings")]
    public float rangeLimit = 3f;

    [Tooltip("0 = Instant, 0.1 = Snappy, 0.3 = Lazy")]
    [Range(0f, 0.5f)]
    public float smoothTime = 0.1f; // Controls the delay/smoothness

    // Internal variable for the math to work
    private Vector3 currentVelocity;

    void Start()
    {
        if (mainCam == null) mainCam = Camera.main;
    }

    void Update()
    {
        // 1. Find where the mouse is
        Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        Vector3 targetPosition = mouseWorldPos;

        // 2. Apply the "Leash" Limit
        if (player != null)
        {
            Vector3 offset = mouseWorldPos - player.position;

            // If mouse is too far, clamp it to the limit
            if (offset.magnitude > rangeLimit)
            {
                offset = Vector3.ClampMagnitude(offset, rangeLimit);
            }

            // Calculate the exact spot we WANT to be
            targetPosition = player.position + offset;
        }

        // 3. Smoothly move towards that spot
        // SmoothDamp handles the acceleration/deceleration automatically
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);
    }
}