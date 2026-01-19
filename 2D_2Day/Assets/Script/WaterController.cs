using UnityEngine;

public class WaterController : MonoBehaviour
{
    [Header("Settings")]
    public float springConstant = 0.02f;
    public float damping = 0.04f;
    public float spread = 0.05f;
    public int springCount = 100;
    public float width = 2f;      // Reduced width to fit a glass
    public float depth = 2f;      // Reduced depth to fit a glass

    [Header("Rendering")]
    public Material waterMaterial; // Don't forget to assign this!

    // Arrays
    float[] yLocalPositions; // Changed to calculate Local Height only
    float[] velocities;
    float[] accelerations;

    // Mesh
    GameObject meshObject;
    Mesh mesh;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;

    void Start()
    {
        InitializePhysics();
        GenerateMesh();

        // Create Trigger Collider automatically
        BoxCollider2D col = gameObject.AddComponent<BoxCollider2D>();
        col.size = new Vector2(width, depth);
        // Offset assumes top-left pivot, moving to center of volume
        col.offset = new Vector2(width / 2f, -depth / 2f);
        col.isTrigger = true;
    }

    void InitializePhysics()
    {
        yLocalPositions = new float[springCount];
        velocities = new float[springCount];
        accelerations = new float[springCount];

        // We don't need xPositions array anymore, we calculate X on the fly
        // We set initial Y to 0 (Local center)
        for (int i = 0; i < springCount; i++)
        {
            yLocalPositions[i] = 0f;
        }
    }

    void GenerateMesh()
    {
        meshObject = new GameObject("WaterMesh");
        meshObject.transform.SetParent(transform, false); // Attach to this object
        meshObject.transform.localPosition = Vector3.zero; // Reset position

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
        // 1. Spring Physics (Local Space)
        for (int i = 0; i < springCount; i++)
        {
            // Target height is always 0 (Local)
            float force = springConstant * (yLocalPositions[i] - 0f) + velocities[i] * damping;
            accelerations[i] = -force;
            yLocalPositions[i] += velocities[i];
            velocities[i] += accelerations[i];
        }

        // 2. Wave Spreading
        float[] leftDeltas = new float[springCount];
        float[] rightDeltas = new float[springCount];

        for (int j = 0; j < 8; j++)
        {
            for (int i = 0; i < springCount; i++)
            {
                if (i > 0)
                {
                    leftDeltas[i] = spread * (yLocalPositions[i] - yLocalPositions[i - 1]);
                    velocities[i - 1] += leftDeltas[i];
                }
                if (i < springCount - 1)
                {
                    rightDeltas[i] = spread * (yLocalPositions[i] - yLocalPositions[i + 1]);
                    velocities[i + 1] += rightDeltas[i];
                }
            }

            for (int i = 0; i < springCount; i++)
            {
                if (i > 0) yLocalPositions[i - 1] += leftDeltas[i];
                if (i < springCount - 1) yLocalPositions[i + 1] += rightDeltas[i];
            }
        }
    }

    void UpdateMesh()
    {
        Vector3[] vertices = new Vector3[springCount * 2];
        int[] triangles = new int[(springCount - 1) * 6];

        float spacing = width / (springCount - 1);

        for (int i = 0; i < springCount; i++)
        {
            float localX = i * spacing;

            // Top Vertex (The Wave)
            vertices[i] = new Vector3(localX, yLocalPositions[i], 0);

            // Bottom Vertex (The Deep End)
            vertices[i + springCount] = new Vector3(localX, -depth, 0);
        }

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
        mesh.RecalculateBounds(); // Important for visibility when moving!
    }

    // Splash uses World Position input, converts to Local Index
    public void Splash(float worldXPos, float velocity)
    {
        // Convert World X to Local X
        float localX = transform.InverseTransformPoint(new Vector3(worldXPos, 0, 0)).x;

        if (localX >= 0 && localX <= width)
        {
            float percentage = localX / width;
            int index = Mathf.RoundToInt(percentage * (springCount - 1));

            // Clamp index to be safe
            index = Mathf.Clamp(index, 0, springCount - 1);

            velocities[index] = velocity;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Unity 6 use linearVelocity, Older Unity use velocity
#if UNITY_6000_0_OR_NEWER
            float speed = rb.linearVelocity.y * rb.mass / 40f;
#else
            float speed = rb.velocity.y * rb.mass / 40f;
#endif

            Splash(collision.transform.position.x, speed);
        }
    }
}