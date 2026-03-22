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
public class DialogueMiniGameSettings
{
    [Tooltip("ถ้าเปิดใช้ จะตัดเข้ามินิเกมรินเหล้าหลังจากพิมพ์บรรทัดนี้จบ (ไม่มีปุ่มให้กดเพิ่ม)")]
    public bool triggerPourLiquorMiniGame = false;

    [Tooltip("ดีเลย์เวลาก่อนตัดเข้า UI มินิเกม (วินาที)")]
    public float delayBeforeStart = 0.5f;

    [Tooltip("ตั้งค่ามินิเกมรินเหล้าสำหรับบรรทัดนี้")]
    public PourLiquorSettings pourLiquorSettings;

    [Tooltip("Dialogue ที่จะเล่นต่อเมื่อมินิเกม 'ชนะ' (ถ้าไม่ตั้ง จะไปบรรทัดถัดไปของ Dialogue เดิม)")]
    public DialoguesObject onMiniGameSuccessDialogue;

    [Tooltip("Dialogue ที่จะเล่นต่อเมื่อมินิเกม 'แพ้' (ถ้าไม่ตั้ง จะไปบรรทัดถัดไปของ Dialogue เดิม)")]
    public DialoguesObject onMiniGameFailDialogue;

    [Tooltip("ผลลัพธ์/ผลข้างเคียงเมื่อมินิเกม 'ชนะ' เช่น ปรับค่า StoryFlag")]
    public List<StoryAction> onMiniGameSuccessActions = new List<StoryAction>();

    [Tooltip("ผลลัพธ์/ผลข้างเคียงเมื่อมินิเกม 'แพ้'")]
    public List<StoryAction> onMiniGameFailActions = new List<StoryAction>();
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

    [Header("Mini Game (Pour Liquor)")]
    public DialogueMiniGameSettings miniGameSettings;
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
    public List<DialogueLine> lines = new List<DialogueLine>();
}
