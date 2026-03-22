using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDialogueTrigger : MonoBehaviour
{
    [SerializeField]
    public EventData eventData;
    public GameObject DialogueUI;
    [Header("Behavior")]
    [Tooltip("ปิด Collider / Trigger นี้ทันทีเมื่อ Event ถูกทำแล้ว (ไม่ repeat)")]
    public bool disableAfterCompleted = true;

    private bool isPlayerInRange = false;

    private void Update()
    {
        if (!isPlayerInRange || !Input.GetKeyDown(KeyCode.E))
            return;

        if (eventData == null)
        {
            Debug.LogError($"[{name}] EventDialogueTrigger has no EventData assigned.");
            return;
        }

        if (DialogueManager.Instance != null && DialogueManager.Instance.isDialogueActive)
            return;

        if (EventManager.Instance == null)
        {
            Debug.LogError($"[{name}] EventManager.Instance is null.");
            return;
        }

        if (EventManager.Instance.CanTriggerEvent(eventData))
        {
            print("Check");
            EventManager.Instance.TriggerEvent(eventData);
        }
        else
        {
            Debug.Log($"Event '{eventData.eventName}' cannot be triggered at this time.");

            // Disable trigger only when the event is actually completed and non-repeatable.
            bool shouldDisable = disableAfterCompleted &&
                                 !eventData.isRepeatable &&
                                 EventManager.Instance.IsEventCompleted(eventData);
            if (shouldDisable)
            {
                var col = GetComponent<Collider2D>();
                if (col != null) col.enabled = false;
                var col3D = GetComponent<Collider>();
                if (col3D != null) col3D.enabled = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsPlayerCollider(other))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (IsPlayerCollider(other))
        {
            isPlayerInRange = false;
        }
    }

    private bool IsPlayerCollider(Collider2D other)
    {
        if (other == null) return false;

        if (other.CompareTag("Player"))
            return true;

        if (other.GetComponent<PlayerController>() != null)
            return true;

        if (other.GetComponentInParent<PlayerController>() != null)
            return true;

        return false;
    }
}
