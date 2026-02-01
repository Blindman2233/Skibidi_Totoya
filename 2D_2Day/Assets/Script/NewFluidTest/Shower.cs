using UnityEngine;

public class Shower : MonoBehaviour
{
    [Header("References")]
    public GameObject Simulation;
    public GameObject Base_Particle;

    [Header("End Level Events")]
    public GameObject winUI;              // Drag Win Screen here

    [Tooltip("These objects disappear when you win (e.g. In-game UI)")]
    public GameObject[] objectsToDeactivate;

    [Tooltip("These objects appear when you win (e.g. Confetti, Next Button)")]
    public GameObject[] objectsToActivate; // NEW: Array for objects to TURN ON

    [Header("Spawn Settings")]
    public Vector2 init_speed = new Vector2(1.0f, 0.0f);
    public float spawn_rate = 10f;
    public int maxParticles = 1000;

    [Header("Time Limits")]
    public float startDelay = 2.0f;
    public float spawnDuration = 5.0f;
    public float deactivateDelay = 4.0f;   // Wait this long AFTER water stops

    // Internal Timers & Flags
    private float spawnTimer;
    private float currentActiveTime;
    private bool isFinishedSpawning = false;
    private bool levelComplete = false;

    void Start()
    {
        if (Simulation == null) Simulation = GameObject.Find("Simulation");
        if (Base_Particle == null) Base_Particle = GameObject.Find("Base_Particle");

        if (winUI != null) winUI.SetActive(false);

        // NEW: Ensure the "Win Objects" are hidden when the game starts
        if (objectsToActivate != null)
        {
            foreach (GameObject obj in objectsToActivate)
            {
                if (obj != null) obj.SetActive(false);
            }
        }
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
            currentActiveTime += Time.deltaTime;

            if (currentActiveTime >= spawnDuration)
            {
                isFinishedSpawning = true;
            }
            else
            {
                PerformSpawning();
            }
        }

        // 3. Handle End Delay (Runs only after water stops)
        if (isFinishedSpawning && !levelComplete)
        {
            deactivateDelay -= Time.deltaTime;

            if (deactivateDelay <= 0)
            {
                TriggerEndEvents();
            }
        }
    }

    void PerformSpawning()
    {
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

    void TriggerEndEvents()
    {
        levelComplete = true;
        Debug.Log("Time is up! Switching objects...");

        // 1. Turn OFF objects (Deactivate)
        if (objectsToDeactivate != null)
        {
            foreach (GameObject obj in objectsToDeactivate)
            {
                if (obj != null) obj.SetActive(false);
            }
        }

        // 2. Turn ON objects (Activate) - NEW!
        if (objectsToActivate != null)
        {
            foreach (GameObject obj in objectsToActivate)
            {
                if (obj != null) obj.SetActive(true);
            }
        }

        // 3. Show Win UI
        if (winUI != null)
        {
            winUI.SetActive(true);
        }
    }
}