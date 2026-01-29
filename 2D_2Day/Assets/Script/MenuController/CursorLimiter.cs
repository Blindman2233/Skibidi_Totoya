using UnityEngine;

public class CursorLimiter : MonoBehaviour
{
    [Header("Settings")]
    public Transform player;       // Drag your Player object here
    public float maxDistance = 3f; // How far the object can go from player

    void Update()
    {
        if (player == null) return;

        // 1. Get the raw mouse position in the world
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0; // Force Z to 0 so it stays on the 2D plane

        // 2. Calculate the direction from the Player to the Mouse
        Vector3 direction = mousePos - player.position;

        // 3. "Clamp" the length. 
        // If the distance is greater than maxDistance, it shortens it.
        // If it's shorter, it leaves it alone.
        Vector3 limitedDirection = Vector3.ClampMagnitude(direction, maxDistance);

        // 4. Set the final position relative to the player
        transform.position = player.position + limitedDirection;
    }
}