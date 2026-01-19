using UnityEngine;

public class Watershooter : MonoBehaviour
{
    public GameObject blueCircle;

    [Header("Speed Settings")]
    [Tooltip("Time in seconds between shots. 0.1 = fast, 0.5 = slow")]
    public float shootDelay = 0.1f;

    [Header("New Settings")]
    [Tooltip("How many seconds to wait before the gun starts shooting")]
    public float startDelay = 2.0f;

    [Tooltip("Total amount of water drops to shoot")]
    public int amountToDrop = 50;

    private float nextShootTime = 0f;
    private int currentDrops = 0;
    private bool isFinished = false; // To make sure we only print "Complete" once

    void Start()
    {
        nextShootTime = Time.time + startDelay;
    }

    void Update()
    {
        // Safety Check: If we are already done, do nothing
        if (isFinished) return;

        if (Time.time > nextShootTime)
        {
            if (currentDrops < amountToDrop)
            {
                Shoot();
                nextShootTime = Time.time + shootDelay;
            }
        }
    }

    void Shoot()
    {
        // 1. Spawn & Shoot
        GameObject drop = Instantiate(blueCircle, transform.position, Quaternion.identity);
        currentDrops++; // Count it!

        Rigidbody2D rb = drop.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.AddForce(transform.right * 1f, ForceMode2D.Impulse);
        }

        Destroy(drop, 30f);

        // 2. CHECK: Did we just shoot the last one?
        if (currentDrops >= amountToDrop)
        {
            Debug.Log("Complete"); // <--- This prints to your Console!
            isFinished = true;     // Stop the gun properly
        }
    }
}