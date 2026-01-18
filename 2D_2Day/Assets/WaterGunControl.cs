using UnityEngine;

public class WaterGunControl : MonoBehaviour
{
    [Header("Fluid Settings")]
    public GameObject waterParticlePrefab; // Drag your Blue Circle PREFAB here
    public float spawnRate = 0.02f;        // How fast it shoots (lower = faster)
    public float shootForce = 10f;         // How hard it shoots
    public float spreadAmount = 0.5f;      // Randomness to make it look like fluid
    public float particleLifetime = 5f;    // Destroy particle after 5 sec to fix lag

    private float timer;

    void Update()
    {
        // Shoots when you hold down the Left Mouse Button
        if (Input.GetMouseButton(0))
        {
            if (Time.time > timer)
            {
                Shoot();
                timer = Time.time + spawnRate;
            }
        }
    }

    void Shoot()
    {
        // 1. Determine spawn position (add slight randomness for "spray" effect)
        Vector3 randomOffset = Random.insideUnitCircle * 0.2f;
        Vector3 spawnPos = transform.position + randomOffset;

        // 2. Spawn the particle
        GameObject drop = Instantiate(waterParticlePrefab, spawnPos, Quaternion.identity);

        // 3. Add Force
        Rigidbody2D rb = drop.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Calculate direction: The red arrow in your image is 'transform.right'
            // We add random y-spread to make it spray out
            Vector2 dir = transform.right + (transform.up * Random.Range(-spreadAmount, spreadAmount));
            rb.AddForce(dir.normalized * shootForce, ForceMode2D.Impulse);
        }

        // 4. Clean up (Destroy old particles so Unity doesn't crash)
        Destroy(drop, particleLifetime);
    }
}