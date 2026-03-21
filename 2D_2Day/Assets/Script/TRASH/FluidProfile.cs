using UnityEngine;

[CreateAssetMenu(fileName = "New Fluid", menuName = "Fluid System/Fluid Profile")]
public class FluidProfile : ScriptableObject
{
    [Header("Visuals")]
    public GameObject prefab;       // The Blue Circle or Red Lava Ball

    [Header("Performance")]
    public int maxPoolSize = 100;   // Max objects (to stop lag)
    public float lifeTime = 3f;     // How long they last

    [Header("Physics")]
    public float fireRate = 0.05f;  // Speed of shooting
    public float shootForce = 20f;  // Power
    public float spread = 0.5f;     // Spray randomness
}