using UnityEngine;

public class Shower : MonoBehaviour
{
    [Header("References")]
    public GameObject Simulation;
    public GameObject Base_Particle;
    public GameObject winUI; // Drag your Win Screen/Panel here!

    [Header("Spawn Settings")]
    public Vector2 init_speed = new Vector2(1.0f, 0.0f);
    public float spawn_rate = 10f;
    public int maxParticles = 1000;

    [Header("Time Limits")]
    public float startDelay = 2.0f;    // Wait before starting
    public float spawnDuration = 5.0f; // How long to spawn water
    public float winDelay = 3.0f;      // How long to wait AFTER water stops before showing Win UI

    // Internal Timers & Flags
    private float spawnTimer;
    private float currentActiveTime;
    private bool isFinishedSpawning = false; // Tracks if water is done
    private bool hasWon = false;             // Tracks if we already showed the UI

    void Start()
    {
        if (Simulation == null) Simulation = GameObject.Find("Simulation");
        if (Base_Particle == null) Base_Particle = GameObject.Find("Base_Particle");

        // Make sure Win UI is hidden when level starts
        if (winUI != null) winUI.SetActive(false);
    }

    void Update()
    {
        // 1. Handle Start Delay
        if (startDelay > 0)
        {
            startDelay -= Time.deltaTime;
            return;
        }

        // 2. Handle Spawning Phase
        if (!isFinishedSpawning)
        {
            // Count up how long we have been spawning
            currentActiveTime += Time.deltaTime;

            // Check if time is up
            if (currentActiveTime >= spawnDuration)
            {
                isFinishedSpawning = true; // Mark as done!
            }
            else
            {
                // Run the standard spawn logic
                PerformSpawning();
            }
        }

        // 3. Handle Win Phase (Only runs after spawning is done)
        if (isFinishedSpawning && !hasWon)
        {
            // Countdown the 3 seconds
            winDelay -= Time.deltaTime;

            if (winDelay <= 0)
            {
                ShowWinScreen();
            }
        }
    }

    void PerformSpawning()
    {
        // Safety check
        if (Simulation != null && Simulation.transform.childCount < maxParticles)
        {
            spawnTimer += Time.deltaTime;
            float interval = 1.0f / spawn_rate;

            if (spawnTimer >= interval)
            {
                SpawnParticle();
                spawnTimer = 0f;
            }
        }
    }

    void SpawnParticle()
    {
        GameObject new_particle = Instantiate(Base_Particle, transform.position, Quaternion.identity);
        Particle pScript = new_particle.GetComponent<Particle>();

        if (pScript != null)
        {
            pScript.pos = transform.position;
            pScript.previous_pos = transform.position;
            pScript.visual_pos = transform.position;
            pScript.vel = init_speed;
        }

        new_particle.transform.parent = Simulation.transform;
    }

    void ShowWinScreen()
    {
        hasWon = true; // Ensure this only happens once
        Debug.Log("Level Complete!");

        if (winUI != null)
        {
            winUI.SetActive(true);
        }
    }
}