using UnityEngine;

public class Watershooter : MonoBehaviour
{
    public GameObject blueCircle;

    [Header("Speed Settings")]
    [Tooltip("Time in seconds between shots. 0.1 = fast, 0.5 = slow")]
    public float shootDelay = 0.1f; // CHANGE THIS NUMBER to control speed!

    private float nextShootTime = 0f;

    void Update()
    {
        // Check if enough time has passed since the last shot
        if (Time.time > nextShootTime)
        {
            Shoot();
            // Reset the timer for the next shot
            nextShootTime = Time.time + shootDelay;
        }
    }

    void Shoot()
    {
        // 1. Spawn the object
        GameObject drop = Instantiate(blueCircle, transform.position, Quaternion.identity);

        // 2. Shoot it to the Right
        Rigidbody2D rb = drop.GetComponent<Rigidbody2D>();
        rb.AddForce(transform.right * 1f, ForceMode2D.Impulse);

        // 3. Destroy it after 20 seconds
        Destroy(drop, 30f);
    }
}