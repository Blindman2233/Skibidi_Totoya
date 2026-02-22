using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StoryCondition
{
    public string flagName;
    public enum ConditionType { Boolean, Integer, Float, String }
    public ConditionType conditionType;
    
    public enum Comparison { Equals, NotEquals, GreaterThan, LessThan, GreaterOrEqual, LessOrEqual }
    public Comparison comparison;
    
    public bool boolValue;
    public int intValue;
    public float floatValue;
    public string stringValue;
    
    public bool CheckCondition(List<StoryFlag> flags)
    {
        StoryFlag flag = flags.Find(f => f.flagName == flagName);
        if (flag == null) return false;
        
        switch (conditionType)
        {
            case ConditionType.Boolean:
                return CompareValues(flag.isActive, boolValue, comparison);
            case ConditionType.Integer:
                return CompareValues(flag.intValue, intValue, comparison);
            case ConditionType.Float:
                return CompareValues(flag.floatValue, floatValue, comparison);
            case ConditionType.String:
                return CompareValues(flag.stringValue, stringValue, comparison);
            default:
                return false;
        }
    }
    
    private bool CompareValues<T>(T value1, T value2, Comparison comp) where T : System.IComparable<T>
    {
        int comparison = value1.CompareTo(value2);
        
        switch (comp)
        {
            case Comparison.Equals: return comparison == 0;
            case Comparison.NotEquals: return comparison != 0;
            case Comparison.GreaterThan: return comparison > 0;
            case Comparison.LessThan: return comparison < 0;
            case Comparison.GreaterOrEqual: return comparison >= 0;
            case Comparison.LessOrEqual: return comparison <= 0;
            default: return false;
        }
    }
}

[CreateAssetMenu(fileName = "EventData", menuName = "Story System/Event Data")]
public class EventData : ScriptableObject
{
    public string eventName;
    public string eventDescription;
    
    [Header("Conditions")]
    public List<StoryCondition> requiredConditions = new List<StoryCondition>();
    
    [Header("Actions")]
    public List<StoryAction> actionsOnComplete = new List<StoryAction>();
    
    [Header("Dialogue")]
    public DialoguesObject dialogueData;
    public bool isRepeatable = false;
    
    [Header("Next Events")]
    public List<EventData> nextEvents = new List<EventData>();
    
    public bool CanTrigger(List<StoryFlag> currentFlags)
    {
        if (requiredConditions.Count == 0) return true;
        
        foreach (var condition in requiredConditions)
        {
            if (!condition.CheckCondition(currentFlags))
                return false;
        }
        
        return true;
    }
}
