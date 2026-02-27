using System.Collections.Generic;
using System;
using UnityEngine;

// Move StoryAction class here to avoid dependency issues
[System.Serializable]
public class StoryAction
{
    public string flagName;
    public enum ActionType { SetBoolean, SetInteger, SetFloat, SetString, Increment, Decrement }
    public ActionType actionType;
    
    public bool boolValue;
    public int intValue;
    public float floatValue;
    public string stringValue;
    
    public void ExecuteAction(List<StoryFlag> flags)
    {
        StoryFlag flag = flags.Find(f => f.flagName == flagName);
        if (flag == null)
        {
            flag = new StoryFlag { flagName = flagName };
            flags.Add(flag);
        }
        
        switch (actionType)
        {
            case ActionType.SetBoolean:
                flag.isActive = boolValue;
                break;
            case ActionType.SetInteger:
                flag.intValue = intValue;
                break;
            case ActionType.SetFloat:
                flag.floatValue = floatValue;
                break;
            case ActionType.SetString:
                flag.stringValue = stringValue;
                break;
            case ActionType.Increment:
                flag.intValue++;
                break;
            case ActionType.Decrement:
                flag.intValue--;
                break;
        }
    }
}

[System.Serializable]
public class StoryFlag
{
    public string flagName;
    public bool isActive;
    public int intValue;
    public float floatValue;
    public string stringValue;
}

public enum DialogueSide
{
    Solo,
    Both
}

[System.Serializable]
public class DialogueCharacter
{
    public string name;
    public Sprite icon;
    public Sprite icon2;
}

[System.Serializable]
public class DialogueLine
{
    public DialogueSide side;
    public DialogueCharacter character;
    [TextArea(3, 10)]
    public string line;
    public bool hasChoices = false;
    public List<DialogueChoice> choices = new List<DialogueChoice>();
    internal object customSound;
}

[System.Serializable]
public class DialogueChoice
{
    [TextArea(2, 5)]
    public string choiceText;
    public List<StoryAction> consequences = new List<StoryAction>();
    public DialoguesObject nextDialogue;
    public EventData triggerEvent;
}

[System.Serializable]
public class DialogueText
{
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();
}


[CreateAssetMenu(fileName = "DialoguesObj", menuName = "Dialogue System/Dialogue")]
public class DialoguesObject : ScriptableObject
{
    [Header("Audio Settings")]
    public AudioClip dialogueSound;
    [Range(0.5f, 1.5f)] public float minPitch = 0.9f;
    [Range(0.5f, 1.5f)] public float maxPitch = 1.1f;
    public int soundFrequency = 1;

    [Header("Activation Settings (Object Names)")]
    [Tooltip("Names of objects to ACTIVATE when dialogue STARTS")]
    public string[] activateBefore;
    [Tooltip("Names of objects to ACTIVATE when dialogue ENDS")]
    public string[] activateAfter;

    [Header("Deactivation Settings (Object Names)")]
    [Tooltip("Names of objects to DEACTIVATE when dialogue STARTS")]
    public string[] deactivateBefore;
    [Tooltip("Names of objects to DEACTIVATE when dialogue ENDS")]
    public string[] deactivateAfter;

    public List<DialogueLine> lines = new List<DialogueLine>();
}
