using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FluidShooter : MonoBehaviour
{
    // Drag your "Water Data" or "Lava Data" file here!
    public FluidProfile fluidData;

    private List<GameObject> pool;
    private float nextShootTime;

    void Start()
    {
        InitializePool();
    }

    // If you change the profile while playing, this rebuilds the pool!
    void OnValidate()
    {
        if (Application.isPlaying && pool != null)
        {
            pool.Clear(); // Warning: simple reset for testing
            InitializePool();
        }
    }

    void InitializePool()
    {
        if (fluidData == null) return;

        pool = new List<GameObject>();
        for (int i = 0; i < fluidData.maxPoolSize; i++)
        {
            GameObject obj = Instantiate(fluidData.prefab);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    void Update()
    {
        // 1. Check Data and Timer
        if (fluidData != null && Input.GetMouseButton(0)) // Added Mouse Click back for control
        {
            if (Time.time > nextShootTime)
            {
                Shoot();
                nextShootTime = Time.time + fluidData.fireRate;
            }
        }
    }

    void Shoot()
    {
        // 2. Find a recycled drop
        foreach (GameObject drop in pool)
        {
            if (!drop.activeInHierarchy)
            {
                // Reset Position
                drop.transform.position = transform.position;
                drop.SetActive(true);

                // Reset Physics
                Rigidbody2D rb = drop.GetComponent<Rigidbody2D>();
                rb.linearVelocity = Vector2.zero;

                // Calculate Force using ScriptableObject Data
                float randomY = Random.Range(-fluidData.spread, fluidData.spread);
                Vector2 dir = transform.right + (transform.up * randomY);
                rb.AddForce(dir.normalized * fluidData.shootForce, ForceMode2D.Impulse);

                // Reset Trail (if it exists)
                TrailRenderer trail = drop.GetComponent<TrailRenderer>();
                if (trail != null) trail.Clear();

                // Recycle Later
                StartCoroutine(HideDropLater(drop));
                return;
            }
        }
    }

    IEnumerator HideDropLater(GameObject drop)
    {
        yield return new WaitForSeconds(fluidData.lifeTime);
        drop.SetActive(false);
    }
}