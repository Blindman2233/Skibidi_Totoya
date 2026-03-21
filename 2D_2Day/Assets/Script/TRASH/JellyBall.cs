using UnityEngine;
using System.Collections;

public class JellyBall : MonoBehaviour
{
    public GameObject blueCircle;

    [Header("Water Settings")]
    public float shootDelay = 0.05f; // Fast shooting
    public float speed = 50f;        // How fast it flies
    public float spread = 1f;      // How messy the spray is (0 = laser, 1 = shower)

    private float nextShootTime = 0f;

    void Update()
    {
        if (Time.time > nextShootTime) // Auto-fire check
        {
            Shoot();
            nextShootTime = Time.time + shootDelay;
        }
    }

    void Shoot()
    {
        // 1. Spawn slightly off-center so they don't overlap instantly
        Vector3 randomPos = transform.position + (Vector3)Random.insideUnitCircle * 0.1f;
        GameObject drop = Instantiate(blueCircle, randomPos, Quaternion.identity); 

        // 2. Add "Spread" (Random Up/Down direction)
        // This makes it spray like a hose instead of a laser
        float randomY = Random.Range(-spread, spread);
        Vector2 direction = transform.right + (transform.up * randomY);

        // 3. Shoot it
        Rigidbody2D rb = drop.GetComponent<Rigidbody2D>();
        rb.AddForce(direction.normalized * speed, ForceMode2D.Impulse);

        // 4. Cleanup
        Destroy(drop, 50f);
    }
}