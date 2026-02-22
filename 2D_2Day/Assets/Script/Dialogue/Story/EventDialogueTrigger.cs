using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDialogueTrigger : MonoBehaviour
{
    [SerializeField]
    public EventData eventData;
    public GameObject DialogueUI;
    
    private bool isPlayerInRange = false;
    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && !DialogueManager.Instance.isDialogueActive)
        {
            if (EventManager.Instance.CanTriggerEvent(eventData))
            {
                EventManager.Instance.TriggerEvent(eventData);
            }
            else
            {
                Debug.Log($"Event '{eventData.eventName}' cannot be triggered at this time.");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}
