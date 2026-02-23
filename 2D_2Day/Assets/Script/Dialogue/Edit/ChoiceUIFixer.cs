#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class ChoiceUIFixer : MonoBehaviour
{
    [MenuItem("Tools/Dialogue System/Fix Choice UI")]
    public static void FixChoiceUI()
    {
        Debug.Log("=== Fixing Choice UI Setup ===");
        
        // Find DialogueManager
        GameObject dialogueManagerObj = GameObject.Find("DialogueManager");
        if (dialogueManagerObj == null)
        {
            Debug.LogError("❌ DialogueManager not found in scene!");
            return;
        }
        
        DialogueManager dialogueManager = dialogueManagerObj.GetComponent<DialogueManager>();
        if (dialogueManager == null)
        {
            Debug.LogError("❌ DialogueManager component not found!");
            return;
        }
        
        // Check if choice panel exists
        if (dialogueManager.choicePanel == null)
        {
            Debug.LogWarning("⚠️ Choice Panel not assigned! Creating one...");
            
            // Find DialogueUI
            Transform dialogueUI = dialogueManagerObj.transform.Find("DialogueUI");
            if (dialogueUI != null)
            {
                // Create Choice Panel
                GameObject choicePanel = new GameObject("ChoicePanel");
                choicePanel.transform.SetParent(dialogueUI);
                
                RectTransform panelRect = choicePanel.AddComponent<RectTransform>();
                panelRect.anchorMin = new Vector2(0.5f, 0.5f);
                panelRect.anchorMax = new Vector2(0.5f, 0.5f);
                panelRect.anchoredPosition = Vector2.zero;
                panelRect.sizeDelta = new Vector2(400, 200);
                
                // Add background
                UnityEngine.UI.Image panelBg = choicePanel.AddComponent<UnityEngine.UI.Image>();
                panelBg.color = new Color(0, 0, 0, 0.8f);
                
                // Create Container
                GameObject container = new GameObject("ChoiceButtonContainer");
                container.transform.SetParent(choicePanel.transform);
                
                RectTransform containerRect = container.AddComponent<RectTransform>();
                containerRect.anchorMin = Vector2.zero;
                containerRect.anchorMax = Vector2.one;
                containerRect.offsetMin = new Vector2(20, 20);
                containerRect.offsetMax = new Vector2(-20, -20);
                
                // Assign to DialogueManager
                dialogueManager.choicePanel = choicePanel;
                dialogueManager.choiceButtonContainer = container.transform;
                
                Debug.Log("✅ Created Choice Panel and Container");
            }
        }
        
        // Check if choice button prefab exists
        if (dialogueManager.choiceButtonPrefab == null)
        {
            Debug.LogWarning("⚠️ Choice Button Prefab not assigned! Creating one...");
            CreateChoiceButtonPrefab();
        }
        
        Debug.Log("=== Choice UI Fix Complete! ===");
    }
    
    private static void CreateChoiceButtonPrefab()
    {
        // Create button
        GameObject button = new GameObject("ChoiceButton");
        button.AddComponent<RectTransform>();
        
        // Set button size and position
        RectTransform rect = button.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(360, 50);
        
        // Add button component
        UnityEngine.UI.Button btn = button.AddComponent<UnityEngine.UI.Button>();
        button.AddComponent<CanvasRenderer>();
        
        // Add background image
        UnityEngine.UI.Image bg = button.AddComponent<UnityEngine.UI.Image>();
        bg.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
        
        // Create text
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
        text.fontSize = 16;
        text.color = Color.white;
        
        // Add button colors
        UnityEngine.UI.ColorBlock colors = btn.colors;
        colors.normalColor = new Color(0.2f, 0.2f, 0.2f, 0.9f);
        colors.highlightedColor = new Color(0.3f, 0.3f, 0.3f, 0.9f);
        colors.pressedColor = new Color(0.1f, 0.1f, 0.1f, 0.9f);
        colors.selectedColor = new Color(0.2f, 0.2f, 0.2f, 0.9f);
        btn.colors = colors;
        
        // Save as prefab
        string prefabPath = "Assets/Prefabs/UI/ChoiceButton.prefab";
        System.IO.Directory.CreateDirectory("Assets/Prefabs/UI");
        
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(button, prefabPath);
        Object.DestroyImmediate(button);
        
        Debug.Log("✅ Created Choice Button Prefab");
    }
    
    [MenuItem("Tools/Dialogue System/Test Choice System")]
    public static void TestChoiceSystem()
    {
        Debug.Log("=== Testing Choice System ===");
        
        // Find DialogueManager
        GameObject dialogueManagerObj = GameObject.Find("DialogueManager");
        if (dialogueManagerObj == null)
        {
            Debug.LogError("❌ DialogueManager not found!");
            return;
        }
        
        DialogueManager dialogueManager = dialogueManagerObj.GetComponent<DialogueManager>();
        
        // Check components
        if (dialogueManager.choicePanel == null)
            Debug.LogError("❌ Choice Panel not assigned!");
        else
            Debug.Log("✅ Choice Panel assigned");
            
        if (dialogueManager.choiceButtonPrefab == null)
            Debug.LogError("❌ Choice Button Prefab not assigned!");
        else
            Debug.Log("✅ Choice Button Prefab assigned");
            
        if (dialogueManager.choiceButtonContainer == null)
            Debug.LogError("❌ Choice Button Container not assigned!");
        else
            Debug.Log("✅ Choice Button Container assigned");
        
        Debug.Log("=== Test Complete ===");
    }
}
#endif
