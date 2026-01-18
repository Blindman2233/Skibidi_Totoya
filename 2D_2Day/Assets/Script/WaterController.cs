using UnityEngine;

public class WaterController : MonoBehaviour
{
    [Header("Settings")]
    public float springConstant = 0.02f;  // How stiff the spring is
    public float damping = 0.04f;         // How quickly waves stop moving
    public float spread = 0.05f;          // How much waves spread to neighbors
    public int springCount = 100;         // Number of points on top surface
    public float width = 10f;             // Total width of the water
    public float depth = 5f;              // Depth of the water

    [Header("Rendering")]
    public Material waterMaterial;

    // Internal physics arrays
    float[] xPositions;
    float[] yPositions;
    float[] velocities;
    float[] accelerations;

    // Mesh components
    LineRenderer body; // For the surface line (optional)
    GameObject meshObject;
    Mesh mesh;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;

    // Impact tracking
    float baseHeight;
    float left;
    float bottom;

    void Start()
    {
        InitializePhysics();
        GenerateMesh();

        // Add a BoxCollider2D trigger so we detect objects entering
        BoxCollider2D col = gameObject.AddComponent<BoxCollider2D>();
        col.size = new Vector2(width, depth);
        col.offset = new Vector2(width / 2, -depth / 2);
        col.isTrigger = true;
    }

    void InitializePhysics()
    {
        xPositions = new float[springCount];
        yPositions = new float[springCount];
        velocities = new float[springCount];
        accelerations = new float[springCount];

        baseHeight = transform.position.y;
        left = transform.position.x;
        bottom = baseHeight - depth;

        float spacing = width / (springCount - 1);

        for (int i = 0; i < springCount; i++)
        {
            yPositions[i] = baseHeight;
            xPositions[i] = left + (i * spacing);
        }
    }

    void GenerateMesh()
    {
        // Setup Mesh Rendering
        meshObject = new GameObject("WaterMesh");
        meshObject.transform.parent = transform;
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshFilter = meshObject.AddComponent<MeshFilter>();
        meshRenderer.material = waterMaterial;

        mesh = new Mesh();
        meshFilter.mesh = mesh;
    }

    void Update()
    {
        UpdatePhysics();
        UpdateMesh();
    }

    void UpdatePhysics()
    {
        // 1. Spring Physics (Hooke's Law)
        for (int i = 0; i < springCount; i++)
        {
            float force = springConstant * (yPositions[i] - baseHeight) + velocities[i] * damping;
            accelerations[i] = -force;
            yPositions[i] += velocities[i];
            velocities[i] += accelerations[i];
        }

        // 2. Wave Spreading (Neighbor interaction)
        float[] leftDeltas = new float[springCount];
        float[] rightDeltas = new float[springCount];

        // Run this spread pass a few times (8 is standard) for smoothness
        for (int j = 0; j < 8; j++)
        {
            for (int i = 0; i < springCount; i++)
            {
                if (i > 0)
                {
                    leftDeltas[i] = spread * (yPositions[i] - yPositions[i - 1]);
                    velocities[i - 1] += leftDeltas[i];
                }
                if (i < springCount - 1)
                {
                    rightDeltas[i] = spread * (yPositions[i] - yPositions[i + 1]);
                    velocities[i + 1] += rightDeltas[i];
                }
            }

            for (int i = 0; i < springCount; i++)
            {
                if (i > 0) yPositions[i - 1] += leftDeltas[i];
                if (i < springCount - 1) yPositions[i + 1] += rightDeltas[i];
            }
        }
    }

    void UpdateMesh()
    {
        Vector3[] vertices = new Vector3[springCount * 2];
        int[] triangles = new int[(springCount - 1) * 6];

        // Build vertices: Top row (springs) and Bottom row (fixed)
        for (int i = 0; i < springCount; i++)
        {
            // Top Vertex (Dynamic)
            vertices[i] = transform.InverseTransformPoint(new Vector3(xPositions[i], yPositions[i], 0));
            // Bottom Vertex (Fixed)
            vertices[i + springCount] = transform.InverseTransformPoint(new Vector3(xPositions[i], bottom, 0));
        }

        // Build Triangles
        int t = 0;
        for (int i = 0; i < springCount - 1; i++)
        {
            int topCurrent = i;
            int topNext = i + 1;
            int bottomCurrent = i + springCount;
            int bottomNext = i + 1 + springCount;

            triangles[t++] = topCurrent;
            triangles[t++] = bottomCurrent;
            triangles[t++] = topNext;

            triangles[t++] = topNext;
            triangles[t++] = bottomCurrent;
            triangles[t++] = bottomNext;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    // Call this from other scripts to make a splash!
    public void Splash(float xpos, float velocity)
    {
        // Find the spring closest to the impact position
        if (xpos >= xPositions[0] && xpos <= xPositions[springCount - 1])
        {
            // Map xpos to array index
            float percentage = (xpos - xPositions[0]) / width;
            int index = Mathf.RoundToInt(percentage * (springCount - 1));

            // Apply force
            velocities[index] = velocity;
        }
    }

    // Automatic splash detection
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Use the falling object's mass and velocity to determine splash size
            float speed = rb.linearVelocity.y * rb.mass / 40f;
            Splash(collision.transform.position.x, speed);
        }
    }
}