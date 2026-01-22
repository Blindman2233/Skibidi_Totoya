using UnityEngine;
using TMPro;

public class FailZone : MonoBehaviour
{
    [Header("Settings")]
    public int maxAllowedDrops = 5;

    [Header("Optional UI")]
    public GameObject failTextObject;

    private int dropCount = 0;
    private bool hasFailed = false; 

    void Start()
    {
        // CRITICAL: Always unpause the game when the level starts!
        // If we don't do this, the game will stay frozen after you restart.
        Time.timeScale = 1f;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasFailed) return;

        if (collision.gameObject.CompareTag("Water"))
        {
            dropCount++;
            Debug.Log("Water touched floor! Count: " + dropCount + " / " + maxAllowedDrops);

            if (dropCount > maxAllowedDrops)
            {
                FailMission();
            }
        }
    }

    void FailMission()
    {
        hasFailed = true;
        Debug.Log("❌ MISSION FAILED: Too much water spilled!");

        // Show the UI
        if (failTextObject != null)
        {
            failTextObject.SetActive(true);
        }

        // --- PAUSE THE GAME ---
        // This stops physics, inputs, and movement immediately
        Time.timeScale = 0f;
    }
}