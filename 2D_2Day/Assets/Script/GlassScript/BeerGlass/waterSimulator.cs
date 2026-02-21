using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class WaterSimulator : MonoBehaviour
{
    [Header("Water Properties")]
    public int resolution = 50; // Number of nodes in the water surface
    public float springConstant = 0.02f;
    public float damping = 0.04f;
    public float spread = 0.05f;

    [Header("Container")]
    [Tooltip("The radius of the circular container.")]
    public float radius = 5f;
    [Tooltip("The fill level of the container, from 0 (empty) to 1 (full).")]
    [Range(0f, 1f)]
    public float waterLevel = 0.5f;

    private float[] yPositions;
    private float[] velocities;
    private float[] accelerations;

    private Mesh waterMesh;
    private Vector3[] vertices;
    private float restingWaterY;

    void Start()
    {
        InitializeWater();
    }

    void Update()
    {
        UpdateWaterPhysics();
        UpdateWaterMesh();
    }

    void InitializeWater()
    {
        yPositions = new float[resolution];
        velocities = new float[resolution];
        accelerations = new float[resolution];
        vertices = new Vector3[resolution * 2];

        // Calculate the resting Y position based on the water level and radius
        restingWaterY = transform.position.y - radius + (waterLevel * radius * 2);

        for (int i = 0; i < resolution; i++)
        {
            yPositions[i] = restingWaterY;
            velocities[i] = 0;
            accelerations[i] = 0;
        }

        waterMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = waterMesh;

        Material waterMaterial = new Material(Shader.Find("Sprites/Default"));
        waterMaterial.color = new Color(0.0f, 0.5f, 0.7f, 0.5f);
        GetComponent<MeshRenderer>().material = waterMaterial;

        UpdateWaterMesh();
    }

    void UpdateWaterPhysics()
    {
        for (int i = 0; i < resolution; i++)
        {
            // The force now pulls the water back to the restingWaterY
            float force = springConstant * (yPositions[i] - restingWaterY) + velocities[i] * damping;
            accelerations[i] = -force;

            yPositions[i] += velocities[i];
            velocities[i] += accelerations[i];
        }

        float[] leftDeltas = new float[resolution];
        float[] rightDeltas = new float[resolution];

        for (int j = 0; j < 8; j++)
        {
            for (int i = 0; i < resolution; i++)
            {
                if (i > 0)
                {
                    leftDeltas[i] = spread * (yPositions[i] - yPositions[i - 1]);
                    velocities[i - 1] += leftDeltas[i];
                }
                if (i < resolution - 1)
                {
                    rightDeltas[i] = spread * (yPositions[i] - yPositions[i + 1]);
                    velocities[i + 1] += rightDeltas[i];
                }
            }

            for (int i = 0; i < resolution; i++)
            {
                if (i > 0)
                    yPositions[i - 1] += leftDeltas[i];
                if (i < resolution - 1)
                    yPositions[i + 1] += rightDeltas[i];
            }
        }
    }

    void UpdateWaterMesh()
    {
        float startX = transform.position.x - radius;
        float width = radius * 2;

        for (int i = 0; i < resolution; i++)
        {
            float x = startX + ((float)i / (resolution - 1)) * width;
            float x_rel = x - transform.position.x;

            // Constrain the water surface to stay within the top semi-circle of the container
            float max_y = transform.position.y + Mathf.Sqrt(Mathf.Max(0f, radius * radius - x_rel * x_rel));
            if (yPositions[i] > max_y)
            {
                yPositions[i] = max_y;
            }

            // Top vertex is the simulated water surface
            vertices[i] = new Vector3(x, yPositions[i], transform.position.z);

            // Bottom vertex follows the curve of the circular container
            float y_bottom = transform.position.y - Mathf.Sqrt(Mathf.Max(0f, radius * radius - x_rel * x_rel));
            vertices[i + resolution] = new Vector3(x, y_bottom, transform.position.z);
        }

        int[] triangles = new int[(resolution - 1) * 6];
        for (int i = 0; i < resolution - 1; i++)
        {
            triangles[i * 6] = i;
            triangles[i * 6 + 1] = i + 1;
            triangles[i * 6 + 2] = i + resolution;

            triangles[i * 6 + 3] = i + 1;
            triangles[i * 6 + 4] = i + resolution + 1;
            triangles[i * 6 + 5] = i + resolution;
        }

        waterMesh.vertices = vertices;
        waterMesh.triangles = triangles;
        waterMesh.RecalculateNormals();
    }

    public void Splash(float xPosition, float speed)
    {
        // Adjust index calculation to account for the radius-based positioning
        int index = Mathf.RoundToInt(((xPosition - (transform.position.x - radius)) / (radius * 2)) * (resolution - 1));
        if (index >= 0 && index < resolution)
        {
            velocities[index] = speed;
        }
    }
}
