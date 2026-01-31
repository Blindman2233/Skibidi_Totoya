using UnityEngine;

public class Shower : MonoBehaviour
{
    [Header("References")]
    public GameObject Simulation;
    public GameObject Base_Particle;

    [Header("Spawn Settings")]
    public Vector2 init_speed = new Vector2(1.0f, 0.0f);
    public float spawn_rate = 10f; // How many particles per second
    public int maxParticles = 1000;

    [Header("Time Limits")]
    public float startDelay = 2.0f;    // How many seconds to WAIT before starting
    public float spawnDuration = 5.0f; // How long to KEEP spawning (0 = Infinite)

    private float spawnTimer;
    private float currentActiveTime;
    private bool isSpawning = false;

    void Start()
    {
        // Fallback: Find objects if they weren't dragged in Inspector
        if (Simulation == null) Simulation = GameObject.Find("Simulation");
        if (Base_Particle == null) Base_Particle = GameObject.Find("Base_Particle");
    }

    void Update()
    {
        // 1. Handle the Start Delay
        if (startDelay > 0)
        {
            startDelay -= Time.deltaTime;
            return; // Stop here, don't spawn yet
        }

        // 2. Check if we are within the allowed Duration
        if (spawnDuration > 0)
        {
            currentActiveTime += Time.deltaTime;
            if (currentActiveTime >= spawnDuration)
            {
                // Time is up! Stop spawning.
                return;
            }
        }

        // 3. Spawn Logic
        // Check if we have room for more particles
        if (Simulation != null && Simulation.transform.childCount < maxParticles)
        {
            spawnTimer += Time.deltaTime;

            // Calculate time between spawns (e.g., 10 spawn_rate = 0.1s interval)
            float interval = 1.0f / spawn_rate;

            if (spawnTimer >= interval)
            {
                SpawnParticle();
                spawnTimer = 0f; // Reset timer cleanly
            }
        }
    }

    void SpawnParticle()
    {
        // Create the particle
        GameObject new_particle = Instantiate(Base_Particle, transform.position, Quaternion.identity);

        // Get the script ONLY ONCE (this is faster/better for performance)
        Particle pScript = new_particle.GetComponent<Particle>();

        if (pScript != null)
        {
            pScript.pos = transform.position;
            pScript.previous_pos = transform.position;
            pScript.visual_pos = transform.position;
            pScript.vel = init_speed;
        }

        // Organize hierarchy
        new_particle.transform.parent = Simulation.transform;
    }
}