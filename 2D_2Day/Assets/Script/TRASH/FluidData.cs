using UnityEngine;

[CreateAssetMenu(fileName = "New Fluid Data", menuName = "Fluid/Fluid Data")]
public class FluidData : ScriptableObject
{
    [Header("Visuals")]
    public GameObject prefab;       // The Blue Cube Prefab

    [Header("Safety Settings")]
    public int maxParticles = 1500; // Stop shooting if we hit this number

    [Header("Physics")]
    public float fireRate = 0.02f;  // Speed of fire
    public float shootForce = 15f;  // Power
    public float spread = 0.5f;     // Messy spray amount
}