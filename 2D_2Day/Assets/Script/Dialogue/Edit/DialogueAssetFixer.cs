#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public class DialogueAssetFixer : EditorWindow
{
    [MenuItem("Tools/Dialogue System/Fix ScriptableObjects")]
    public static void FixScriptableObjects()
    {
        Debug.Log("=== Fixing Dialogue ScriptableObjects ===");
        
        // Find all Dialogue assets
        string[] guids = AssetDatabase.FindAssets("t:DialoguesObject");
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            DialoguesObject dialogue = AssetDatabase.LoadAssetAtPath<DialoguesObject>(path);
            
            if (dialogue != null)
            {
                Debug.Log($"Found Dialogue: {dialogue.name} at {path}");
                
                // Force reimport to fix script references
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }
        }
        
        // Find all Event assets
        guids = AssetDatabase.FindAssets("t:EventData");
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            EventData eventData = AssetDatabase.LoadAssetAtPath<EventData>(path);
            
            if (eventData != null)
            {
                Debug.Log($"Found Event: {eventData.name} at {path}");
                
                // Force reimport to fix script references
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }
        }
        
        // Refresh AssetDatabase
        AssetDatabase.Refresh();
        
        Debug.Log("=== ScriptableObject Fix Complete! ===");
        Debug.Log("Try reopening your project to see if the issue is resolved.");
    }
    
    [MenuItem("Tools/Dialogue System/Validate System")]
    public static void ValidateSystem()
    {
        Debug.Log("=== Validating Dialogue System ===");
        
        // Check if required classes exist
        bool hasDialogueData = System.Type.GetType("DialoguesObject") != null;
        bool hasEventData = System.Type.GetType("EventData") != null;
        bool hasStoryFlag = System.Type.GetType("StoryFlag") != null;
        bool hasStoryAction = System.Type.GetType("StoryAction") != null;
        
        Debug.Log($"DialoguesObject: {(hasDialogueData ? "✅" : "❌")}");
        Debug.Log($"EventData: {(hasEventData ? "✅" : "❌")}");
        Debug.Log($"StoryFlag: {(hasStoryFlag ? "✅" : "❌")}");
        Debug.Log($"StoryAction: {(hasStoryAction ? "✅" : "❌")}");
        
        if (hasDialogueData && hasEventData && hasStoryFlag && hasStoryAction)
        {
            Debug.Log("✅ All required classes are available!");
        }
        else
        {
            Debug.LogError("❌ Some classes are missing. Check for compile errors!");
        }
        
        // Check for existing assets
        string[] dialogueGuids = AssetDatabase.FindAssets("t:DialoguesObject");
        string[] eventGuids = AssetDatabase.FindAssets("t:EventData");
        
        Debug.Log($"Found {dialogueGuids.Length} Dialogue assets");
        Debug.Log($"Found {eventGuids.Length} Event assets");
        
        Debug.Log("=== Validation Complete ===");
    }
    
    [MenuItem("Tools/Dialogue System/Create Test Dialogue")]
    public static void CreateTestDialogue()
    {
        Debug.Log("Creating Test Dialogue...");
        
        // Create new Dialogue asset
        DialoguesObject dialogue = ScriptableObject.CreateInstance<DialoguesObject>();
        dialogue.name = "TestDialogue_Fixed";
        
        // Add test line
        DialogueLine line = new DialogueLine();
        line.line = "This is a test dialogue after fixing the system.";
        line.hasChoices = false;
        
        dialogue.lines.Add(line);
        
        // Save asset
        string path = "Assets/Script/Dialogue/TestDialogue_Fixed.asset";
        AssetDatabase.CreateAsset(dialogue, path);
        AssetDatabase.SaveAssets();
        
        Debug.Log($"✅ Created Test Dialogue at: {path}");
        
        // Select the asset in Inspector
        Selection.activeObject = dialogue;
        EditorGUIUtility.PingObject(dialogue);
    }
}
#endif
