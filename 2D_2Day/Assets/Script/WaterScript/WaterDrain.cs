using UnityEngine;

public class WaterDrain : MonoBehaviour
{
    // When something enters this object's area
    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object has the tag "Water"
        if (other.CompareTag("Water"))
        {
            // Delete the water object
            Destroy(other.gameObject);
        }
    }

    // (Optional) Backup for solid collisions
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Water"))
        {
            Destroy(collision.gameObject);
        }
    }
}