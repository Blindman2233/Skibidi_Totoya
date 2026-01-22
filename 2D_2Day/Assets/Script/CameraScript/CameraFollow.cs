using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // Drag your Player here

    [Header("Position Settings")]
    // X = Left/Right, Y = Up/Down (Height), Z = Layer Depth
    public Vector3 offset = new Vector3(0f, 1.5f, -10f);

    [Header("Zoom Settings")]
    // 5 is default. Smaller = Closer. Larger = Farther.
    public float cameraSize = 5f;

    [Header("Smoothing Settings")]
    [Range(0, 1)]
    public float smoothTime = 0.2f;

    private Vector3 velocity = Vector3.zero;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>(); // Auto-find the camera component
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            // --- 1. Position Logic (Height & Placement) ---
            Vector3 targetPosition = target.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

            // --- 2. Zoom Logic (Close / Far) ---
            // We use Linear Interpolation (Lerp) to smooth the zoom too
            if (cam.orthographic)
            {
                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, cameraSize, smoothTime);
            }
        }
    }
}