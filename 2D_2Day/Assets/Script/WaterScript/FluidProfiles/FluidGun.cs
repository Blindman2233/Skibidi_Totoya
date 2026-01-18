using UnityEngine;

public class FluidGun : MonoBehaviour
{
    [Header("Configuration")]
    public FluidData data; // Drag your Data File here!

    // We track how many we have created
    private int currentCount = 0;
    private float nextFireTime;

    void Update()
    {
        // 1. Check if Data exists
        if (data == null) return;

        // 2. Input + Timer + Safety Check
        if (Input.GetMouseButton(0) && Time.time > nextFireTime)
        {
            if (currentCount < data.maxParticles)
            {
                Shoot();
                nextFireTime = Time.time + data.fireRate;
            }
            else
            {
                // Optional: Print warning so you know why it stopped
                Debug.LogWarning("Limit Reached! Stopped to save PC.");
            }
        }
    }

    void Shoot()
    {
        // 3. Create the ball (Keep it forever)
        GameObject ball = Instantiate(data.prefab, transform.position, Quaternion.identity);
        currentCount++; // Count it

        // 4. Randomize Visuals (Size & Rotation)
        float randomSize = Random.Range(0.8f, 1.2f);
        ball.transform.localScale = new Vector3(randomSize, randomSize, 1f);
        ball.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));

        // 5. Physics Push
        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();

        float randomY = Random.Range(-data.spread, data.spread);
        Vector2 dir = transform.right + (transform.up * randomY);

        rb.AddForce(dir.normalized * data.shootForce, ForceMode2D.Impulse);
    }
}