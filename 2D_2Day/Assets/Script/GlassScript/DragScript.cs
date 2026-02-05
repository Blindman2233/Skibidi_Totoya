using UnityEngine;

public class SpaceDrag : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("How long it takes to reach the mouse. 0 = Instant, 0.1 = Fast, 0.3 = Slow")]
    public float smoothTime = 0.1f; // Adjust this for more/less lag

    private Vector3 currentVelocity; // Variable needed for the math

    void Update()
    {
        // Check if Left Mouse Button (0) is being HELD down
        if (Input.GetMouseButton(0))
        {
            // 1. Get the mouse position in the world
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = -9.52f; // Keep it on the 2D plane

            // 2. Smoothly move the object towards the mouse
            // SmoothDamp automatically handles the speed up/slow down
            transform.position = Vector3.SmoothDamp(transform.position, mousePos, ref currentVelocity, smoothTime);
        }
    }
}