using UnityEngine;

public class SpaceDrag : MonoBehaviour
{
    void Update()
    {
        // Check if Spacebar is being HELD down
        if (Input.GetKey(KeyCode.Space))
        {
            // 1. Debug log to prove it works
            Debug.Log("Space Key is HELD! Moving object...");

            // 2. Get the mouse position in the world
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // 3. Force Z to 0 (2D games need this)
            mousePos.z = 0;

            // 4. Move the object directly to the mouse
            transform.position = mousePos;
        }
    }
}