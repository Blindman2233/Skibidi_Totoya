using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager : MonoBehaviour, IDataPersistence
{
    public static EventManager Instance;

    [Header("Story State")]
    public List<StoryFlag> storyFlags = new List<StoryFlag>();
    public List<EventData> completedEvents = new List<EventData>();

    [Header("Available Events")]
    public List<EventData> allEvents = new List<EventData>();

    // เก็บ Event ปัจจุบันสำหรับ Next Event
    private EventData currentEvent;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);




        // ฟัง Event จาก DialogueManager

    }

    private void OnEnable()
    {
        if (DataPersistenceManager.Instance != null)
            DataPersistenceManager.Instance.Register(this);
        DialogueManager.OnDialogueEnded += OnDialogueComplete;
        InitializeDefaultFlags();
    }

    private void OnDestroy()
    {
        if (DataPersistenceManager.Instance != null)
            DataPersistenceManager.Instance.Unregister(this);
        DialogueManager.OnDialogueEnded -= OnDialogueComplete;
    }

    private void InitializeDefaultFlags()
    {
        if (storyFlags.Count == 0)
        {
            storyFlags.Add(new StoryFlag { flagName = "Chapter", intValue = 1 });
            storyFlags.Add(new StoryFlag { flagName = "PlayerReputation", intValue = 0 });
            storyFlags.Add(new StoryFlag { flagName = "HasMetMafiaBoss", isActive = false });
            storyFlags.Add(new StoryFlag { flagName = "BarUnlocked", isActive = false });
        }
    }

    public bool CanTriggerEvent(EventData eventData)
    {
        if (eventData == null) return false;

        // ตรวจสอบว่า event นี้เคยเกิดแล้วหรือยัง (ถ้าไม่ใช่ repeatable)
        if (!eventData.isRepeatable && completedEvents.Contains(eventData))
        {
            Debug.Log($"[EventManager] Event '{eventData.eventName}' already completed and not repeatable");
            return false;
        }

        bool canTrigger = eventData.CanTrigger(storyFlags);
        Debug.Log($"[EventManager] Event '{eventData.eventName}' can trigger: {canTrigger}");

        return canTrigger;
    }

    public bool IsEventCompleted(EventData eventData)
    {
        if (eventData == null) return false;
        if (completedEvents.Contains(eventData)) return true;

        // Fallback by name in case references differ after load/reimport.
        return completedEvents.Exists(e =>
            e != null &&
            !string.IsNullOrEmpty(e.eventName) &&
            e.eventName == eventData.eventName);
    }

    public void TriggerEvent(EventData eventData)
    {
        if (!CanTriggerEvent(eventData)) return;

        // เก็บ Event ปัจจุบันสำหรับ Next Event
        currentEvent = eventData;

        Debug.Log($"[EventManager] Triggering event: {eventData.eventName}");

        foreach (var action in eventData.actionsOnComplete)
        {
            if (action != null)
            {
                action.ExecuteAction(storyFlags);
                Debug.Log($"[EventManager] Executed action: {action.actionType} on {action.flagName}");
            }
        }

        switch (eventData.eventType)
        {
            case EventData.EventType.DialogueOnly:
                print("isInsideSwitch");
                if (eventData.dialogueData != null)
                {
                    print("isInsideeventTrigger");
                    if (DialogueManager.Instance != null)
                    {
                        DialogueManager.Instance.StartDialogue(eventData.dialogueData, eventData);
                    }
                    else
                    {
                        Debug.LogError("[EventManager] DialogueManager.Instance is null, cannot start dialogue.");
                    }
                }
                break;
            case EventData.EventType.PourLiquor:
                // TODO: เรียก minigame รินเหล้า
                Debug.Log("Pour Liquor minigame not implemented yet");
                CheckForNewEvents();
                break;
            default:
                print("No");
                break;
        }

        // ทำเครื่องหมายว่า event นี้เสร็จแล้ว (ถ้าไม่ใช่ repeatable)
        if (!eventData.isRepeatable)
        {
            completedEvents.Add(eventData);
            Debug.Log($"[EventManager] Event '{eventData.eventName}' marked as completed");
        }

        Debug.Log($"[EventManager] Total completed events: {completedEvents.Count}");
    }

    private void CheckForNewEvents()
    {
        foreach (var eventData in allEvents)
        {
            if (CanTriggerEvent(eventData) && !completedEvents.Contains(eventData))
            {
                Debug.Log($"New event available: {eventData.eventName}");
            }
        }
    }

    // สำคัญ! ฟังก์ชันนี้จัดการ Next Event หลังจาก Dialogue จบ
    private void OnDialogueComplete(EventData completedEvent)
    {
        if (completedEvent == null) return;

        Debug.Log($"Dialogue complete for: {completedEvent.eventName}");

        // ตรวจสอบว่ามี Next Events หรือไม่
        if (completedEvent.nextEvents != null && completedEvent.nextEvents.Count > 0)
        {
            // ไป Event แรกในลิสต์ (index 0)
            EventData nextEvent = completedEvent.nextEvents[0];
            Debug.Log($"Moving to next event: {nextEvent.eventName}");

            // เรียก Next Event อัตโนมัติ
            TriggerEvent(nextEvent);
        }
        else
        {
            Debug.Log("No next events. Checking for new available events.");
            CheckForNewEvents();
        }

        // ล้างค่า currentEvent ถ้าจบแล้ว
        if (currentEvent == completedEvent)
            currentEvent = null;
    }

    public List<EventData> GetAvailableEvents()
    {
        List<EventData> availableEvents = new List<EventData>();

        foreach (var eventData in allEvents)
        {
            if (CanTriggerEvent(eventData))
            {
                availableEvents.Add(eventData);
            }
        }

        return availableEvents;
    }

    public StoryFlag GetFlag(string flagName)
    {
        return storyFlags.Find(f => f.flagName == flagName);
    }

    public void SetFlag(string flagName, bool value)
    {
        StoryFlag flag = GetFlag(flagName);
        if (flag == null)
        {
            flag = new StoryFlag { flagName = flagName };
            storyFlags.Add(flag);
        }
        flag.isActive = value;
    }

    public void SetFlag(string flagName, int value)
    {
        StoryFlag flag = GetFlag(flagName);
        if (flag == null)
        {
            flag = new StoryFlag { flagName = flagName };
            storyFlags.Add(flag);
        }
        flag.intValue = value;
    }

    public void SetFlag(string flagName, float value)
    {
        StoryFlag flag = GetFlag(flagName);
        if (flag == null)
        {
            flag = new StoryFlag { flagName = flagName };
            storyFlags.Add(flag);
        }
        flag.floatValue = value;
    }

    public void SetFlag(string flagName, string value)
    {
        StoryFlag flag = GetFlag(flagName);
        if (flag == null)
        {
            flag = new StoryFlag { flagName = flagName };
            storyFlags.Add(flag);
        }
        flag.stringValue = value;
    }

    public void SaveStoryState()
    {
        PlayerPrefs.SetString("StoryFlags", JsonUtility.ToJson(new StoryFlagWrapper { flags = storyFlags }));
        PlayerPrefs.Save();
    }

    public void LoadStoryState()
    {
        if (PlayerPrefs.HasKey("StoryFlags"))
        {
            string json = PlayerPrefs.GetString("StoryFlags");
            StoryFlagWrapper wrapper = JsonUtility.FromJson<StoryFlagWrapper>(json);
            storyFlags = wrapper.flags;
        }
    }

    // --- IDataPersistence (Save & Load system) ---
    public void LoadData(GameData data)
    {
        if (data == null) return;
        storyFlags = data.storyFlags ?? new List<StoryFlag>();
        if (storyFlags.Count == 0)
            InitializeDefaultFlags();

        // Clear and reload completed events
        completedEvents.Clear();
        if (data.completedEventNames != null && allEvents != null)
        {
            Debug.Log($"[EventManager] Loading {data.completedEventNames.Count} completed events");
            foreach (string name in data.completedEventNames)
            {
                var ev = allEvents.Find(e => e != null && e.eventName == name);
                if (ev != null)
                {
                    completedEvents.Add(ev);
                    Debug.Log($"[EventManager] Loaded completed event: {name}");
                }
                else
                {
                    Debug.LogWarning($"[EventManager] Event not found in allEvents: {name}");
                }
            }
        }

        Debug.Log($"[EventManager] Total completed events after load: {completedEvents.Count}");
    }

    public void SaveData(ref GameData data)
    {
        if (data == null) return;
        data.storyFlags = storyFlags;
        data.completedEventNames.Clear();
        if (completedEvents != null)
        {
            foreach (var ev in completedEvents)
                if (ev != null)
                {
                    data.completedEventNames.Add(ev.eventName);
                    Debug.Log($"[EventManager] Saving completed event: {ev.eventName}");
                }
        }

        Debug.Log($"[EventManager] Saved {data.completedEventNames.Count} completed events");
    }

    [System.Serializable]
    private class StoryFlagWrapper
    {
        public List<StoryFlag> flags;
    }
}
