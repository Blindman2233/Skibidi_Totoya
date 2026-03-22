using UnityEngine;

[CreateAssetMenu(fileName = "PourLiquorSettings", menuName = "MiniGames/Pour Liquor Settings")]
public class PourLiquorSettings : ScriptableObject
{
    [Header("Timing")]
    [Tooltip("เวลาจำกัดของมินิเกม (วินาที)")]
    public float timeLimit = 5f;

    [Header("Target Fill")]
    [Range(0f, 1f)]
    [Tooltip("ปริมาณที่ลงแก้วแล้วถือว่าชนะ (0-1) เช่น 0.95 = 95%")]
    public float targetFillPercent = 0.95f;

    [Header("Spill Limit")]
    [Range(0f, 1f)]
    [Tooltip("ปริมาณที่หกได้สูงสุด (0-1)")]
    public float maxSpillPercent = 0.1f;

    [Header("Pour Speed")]
    [Tooltip("ความเร็วในการเติมลงแก้วต่อวินาที (0-1)")]
    public float pourSpeed = 0.35f;

    [Tooltip("ความเร็วในการหกต่อวินาที (0-1)")]
    public float spillSpeed = 0.5f;
}

