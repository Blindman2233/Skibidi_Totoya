#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class AutoSetupDialogueSystem : MonoBehaviour
{
    [MenuItem("Tools/Dialogue System/Setup Test Scene")]
    public static void SetupTestScene()
    {
        Debug.Log("=== Setting up Dialogue System Test Scene ===");
        
        // 1. สร้าง EventManager
        GameObject eventManager = GameObject.Find("EventManager");
        if (eventManager == null)
        {
            eventManager = new GameObject("EventManager");
            eventManager.AddComponent<EventManager>();
            Debug.Log("✅ Created EventManager");
        }
        
        // 2. สร้าง Test Dialogue Data
        CreateTestDialogueData();
        
        // 3. สร้าง Test Event Data
        CreateTestEventData();
        
        // 4. สร้าง Test NPC
        CreateTestNPC();
        
        Debug.Log("=== Setup Complete! ===");
        Debug.Log("Press F1-F3 in Play Mode to test the system");
    }
    
    private static void CreateTestDialogueData()
    {
        // สร้าง DialogueData สำหรับทดสอบ
        string path = "Assets/Script/Dialogue/TestDialogue.asset";
        
        DialoguesObject dialogue = ScriptableObject.CreateInstance<DialoguesObject>();
        dialogue.name = "TestDialogue";
        
        // สร้าง Character ทดสอบ
        DialogueCharacter testChar = new DialogueCharacter();
        testChar.name = "Test NPC";
        
        // สร้าง DialogueLine พร้อม Choice
        DialogueLine line = new DialogueLine();
        line.character = testChar;
        line.side = DialogueSide.Solo;
        line.line = "สวัสดี! อยากจะพูดคุยอะไรกับผมไหม?";
        line.hasChoices = true;
        
        // สร้าง Choices
        DialogueChoice choice1 = new DialogueChoice();
        choice1.choiceText = "ถามเรื่องแมฟเฟีย";
        
        DialogueChoice choice2 = new DialogueChoice();
        choice2.choiceText = "ถามเรื่องบาร์";
        
        line.choices.Add(choice1);
        line.choices.Add(choice2);
        
        dialogue.lines.Add(line);
        
        AssetDatabase.CreateAsset(dialogue, path);
        AssetDatabase.SaveAssets();
        
        Debug.Log("✅ Created Test Dialogue Data");
    }
    
    private static void CreateTestEventData()
    {
        string path = "Assets/Script/Story/TestEvent.asset";
        
        EventData eventData = ScriptableObject.CreateInstance<EventData>();
        eventData.name = "TestEvent";
        eventData.eventName = "First Meeting";
        eventData.eventDescription = "การพบกับ NPC ครั้งแรก";
        eventData.isRepeatable = false;
        
        // เพิ่ม Action ให้เปลี่ยน Story Flag
        StoryAction action = new StoryAction();
        action.flagName = "HasMetNPC";
        action.actionType = StoryAction.ActionType.SetBoolean;
        action.boolValue = true;
        
        eventData.actionsOnComplete.Add(action);
        
        AssetDatabase.CreateAsset(eventData, path);
        AssetDatabase.SaveAssets();
        
        Debug.Log("✅ Created Test Event Data");
    }
    
    private static void CreateTestNPC()
    {
        GameObject npc = GameObject.Find("TestNPC");
        if (npc == null)
        {
            npc = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            npc.name = "TestNPC";
            npc.tag = "NPC";
            
            // เพิ่ม Collider
            BoxCollider2D collider = npc.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            
            // เพิ่ม EventDialogueTrigger
            EventDialogueTrigger trigger = npc.AddComponent<EventDialogueTrigger>();
            
            Debug.Log("✅ Created Test NPC with EventDialogueTrigger");
        }
    }
    
    [MenuItem("Tools/Dialogue System/Create Choice Button Prefab")]
    public static void CreateChoiceButtonPrefab()
    {
        // สร้าง Button สำหรับ Choice
        GameObject button = new GameObject("ChoiceButton");
        button.AddComponent<RectTransform>();
        
        UnityEngine.UI.Button btn = button.AddComponent<UnityEngine.UI.Button>();
        button.AddComponent<CanvasRenderer>();
        
        // เพิ่ม Text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(button.transform);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        TMPro.TextMeshProUGUI text = textObj.AddComponent<TMPro.TextMeshProUGUI>();
        text.text = "Choice Text";
        text.alignment = TMPro.TextAlignmentOptions.Center;
        
        // บันทึกเป็น Prefab
        string prefabPath = "Assets/Prefabs/UI/ChoiceButton.prefab";
        System.IO.Directory.CreateDirectory("Assets/Prefabs/UI");
        
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(button, prefabPath);
        Object.DestroyImmediate(button);
        
        Debug.Log("✅ Created Choice Button Prefab");
    }
}
#endif
