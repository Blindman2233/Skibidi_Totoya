using UnityEngine;

public class Shower : MonoBehaviour
{
    [Header("References")]
    public GameObject Simulation;
    public GameObject Base_Particle;
    public GameObject winUI;

    [Header("Point Settings")]
    public int pointsPerDrop = 1; // Points gained for each water particle spawned

    [Header("End Level Events")]
    public GameObject[] objectsToDeactivate;
    public GameObject[] objectsToActivate;

    [Header("Spawn Settings")]
    public Vector2 init_speed = new Vector2(1.0f, 0.0f);
    public float spawn_rate = 10f;
    public int maxParticles = 1000;

    [Header("Time Limits")]
    public float startDelay = 2.0f;
    public float spawnDuration = 5.0f;
    public float deactivateDelay = 4.0f;

    private float spawnTimer;
    private float currentActiveTime;
    private bool isFinishedSpawning = false;
    private bool levelComplete = false;

    void Start()
    {
        if (Simulation == null) Simulation = GameObject.Find("Simulation");
        if (Base_Particle == null) Base_Particle = GameObject.Find("Base_Particle");
        if (winUI != null) winUI.SetActive(false);

        if (objectsToActivate != null)
        {
            foreach (GameObject obj in objectsToActivate) if (obj != null) obj.SetActive(false);
        }
    }

    void Update()
    {
        if (startDelay > 0) { startDelay -= Time.deltaTime; return; }

        if (!isFinishedSpawning)
        {
            currentActiveTime += Time.deltaTime;
            if (currentActiveTime >= spawnDuration) isFinishedSpawning = true;
            else PerformSpawning();
        }

        if (isFinishedSpawning && !levelComplete)
        {
            deactivateDelay -= Time.deltaTime;
            if (deactivateDelay <= 0) TriggerEndEvents();
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

        // --- ADDED SCORE LOGIC ---
        if (ScoreManager.instance != null) ScoreManager.instance.ChangeScore(pointsPerDrop);

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
        if (objectsToDeactivate != null) foreach (GameObject obj in objectsToDeactivate) if (obj != null) obj.SetActive(false);
        if (objectsToActivate != null) foreach (GameObject obj in objectsToActivate) if (obj != null) obj.SetActive(true);
        if (winUI != null) winUI.SetActive(true);
    }
}