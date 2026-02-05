using UnityEngine;

public class WaterController : MonoBehaviour
{
    [Header("Settings")]
    public float springConstant = 0.02f;
    public float damping = 0.04f;
    public float spread = 0.07f;
    public int springCount = 120;
    public float width = 2f;      // Reduced width to fit a glass
    public float depth = 2f;      // Reduced depth to fit a glass

    [Header("Rendering")]
    public Material waterMaterial; // Don't forget to assign this!
    public Color surfaceColor = new Color(0.3f, 0.7f, 1f, 0.9f);
    public Color deepColor = new Color(0.0f, 0.15f, 0.35f, 1f);

    // Arrays
    float[] yLocalPositions; // Changed to calculate Local Height only
    float[] velocities;
    float[] accelerations;

    // Cached buffers to avoid per-frame allocations
    float[] leftDeltas;
    float[] rightDeltas;

    // Mesh
    GameObject meshObject;
    Mesh mesh;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;

    // Cached mesh data arrays
    Vector3[] vertices;
    Vector2[] uvs;
    Color[] colors;
    int[] triangles;

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
        leftDeltas = new float[springCount];
        rightDeltas = new float[springCount];

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
        mesh.MarkDynamic(); // Hint Unity this mesh changes every frame
        meshFilter.mesh = mesh;

        // Allocate mesh data arrays once (no GC each frame)
        if (springCount < 2)
        {
            springCount = 2; // Safety: avoid division by zero / invalid mesh
        }

        vertices = new Vector3[springCount * 2];
        uvs = new Vector2[springCount * 2];
        colors = new Color[springCount * 2];
        triangles = new int[(springCount - 1) * 6];
    }

    void Update()
    {
        UpdatePhysics();
        UpdateMesh();
    }

    void UpdatePhysics()
    {
        float dt = Time.deltaTime;

        // 1. Spring Physics (Local Space)
        for (int i = 0; i < springCount; i++)
        {
            // Target height is always 0 (Local)
            float force = springConstant * (yLocalPositions[i] - 0f) + velocities[i] * damping;
            accelerations[i] = -force;
            velocities[i] += accelerations[i] * dt;
            yLocalPositions[i] += velocities[i] * dt;
        }

        // 2. Wave Spreading (reuse cached buffers to avoid allocations)
        for (int j = 0; j < 8; j++)
        {
            for (int i = 0; i < springCount; i++)
            {
                if (i > 0)
                {
                    leftDeltas[i] = spread * (yLocalPositions[i] - yLocalPositions[i - 1]) * dt;
                    velocities[i - 1] += leftDeltas[i];
                }
                if (i < springCount - 1)
                {
                    rightDeltas[i] = spread * (yLocalPositions[i] - yLocalPositions[i + 1]) * dt;
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
        float spacing = width / (springCount - 1);

        for (int i = 0; i < springCount; i++)
        {
            float localX = i * spacing;
            float u = (float)i / (springCount - 1);

            // Top Vertex (The Wave)
            vertices[i] = new Vector3(localX, yLocalPositions[i], 0);
            uvs[i] = new Vector2(u, 1f);
            colors[i] = surfaceColor;

            // Bottom Vertex (The Deep End)
            vertices[i + springCount] = new Vector3(localX, -depth, 0);
            uvs[i + springCount] = new Vector2(u, 0f);
            colors[i + springCount] = deepColor;
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
        mesh.colors = colors;
        mesh.uv = uvs;
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