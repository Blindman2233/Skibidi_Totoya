using UnityEngine;
using TMPro; // Include this just in case you use TextMeshPro later

public class FailZone : MonoBehaviour
{
    [Header("Optional UI")]
    public GameObject failTextObject; // Drag your "Fail" text here (if you have one)

    private bool hasFailed = false;

    // This detects when something physically bumps into the floor
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Only trigger if we haven't failed yet
        if (hasFailed) return;

        // Check if the object is Water
        // (We need to make sure your Water Prefab has the tag "Water")
        if (collision.gameObject.CompareTag("Water"))
        {
            FailMission();
        }
    }

    void FailMission()
    {
        hasFailed = true;
        Debug.Log("❌ MISSION FAILED: Water touched the floor!");

        // If you attached a UI text, show it now
        if (failTextObject != null)
        {
            failTextObject.SetActive(true);
        }
    }
}