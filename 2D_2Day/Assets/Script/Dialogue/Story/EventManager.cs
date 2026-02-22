using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;
    
    [Header("Story State")]
    public List<StoryFlag> storyFlags = new List<StoryFlag>();
    public List<EventData> completedEvents = new List<EventData>();
    
    [Header("Available Events")]
    public List<EventData> allEvents = new List<EventData>();
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);
        InitializeDefaultFlags();
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
        
        if (!eventData.isRepeatable && completedEvents.Contains(eventData))
            return false;
            
        return eventData.CanTrigger(storyFlags);
    }
    
    public void TriggerEvent(EventData eventData)
    {
        if (!CanTriggerEvent(eventData)) return;
        
        foreach (var action in eventData.actionsOnComplete)
        {
            action.ExecuteAction(storyFlags);
        }
        
        if (!eventData.isRepeatable)
        {
            completedEvents.Add(eventData);
        }
        
        if (eventData.dialogueData != null)
        {
            DialogueManager.Instance.StartDialogue(eventData.dialogueData);
        }
        
        CheckForNewEvents();
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
    
    [System.Serializable]
    private class StoryFlagWrapper
    {
        public List<StoryFlag> flags;
    }
}
