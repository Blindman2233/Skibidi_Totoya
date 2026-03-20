using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Main menu: New Game, Continue, Option, Quit.
/// Continue loads the latest save and then loads the game scene (or just load latest save if already in game).
/// Assign buttons in inspector and set firstGameSceneName if New Game should load a specific scene.
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    [Header("Buttons")]
    public Button newGameButton;
    public Button continueButton;
    public Button optionButton;
    public Button quitButton;

    [Header("Save/Load screen")]
    [Tooltip("SaveLoadUI panel for loading saved games. Required for Continue button to work.")]
    public GameObject saveLoadPanel;

    [Header("Scenes")]
    [Tooltip("Scene to load for New Game (e.g. SampleScene). Leave empty to only call NewGame() and stay in menu scene.")]
    public string firstGameSceneName = "SampleScene";

    private void OnEnable()
    {
        if (newGameButton != null) newGameButton.onClick.RemoveAllListeners();
        newGameButton?.onClick.AddListener(OnNewGame);

        if (continueButton != null) continueButton.onClick.RemoveAllListeners();
        continueButton?.onClick.AddListener(OnContinue);

        if (optionButton != null) optionButton.onClick.RemoveAllListeners();
        optionButton?.onClick.AddListener(OnOption);

        if (quitButton != null) quitButton.onClick.RemoveAllListeners();
        quitButton?.onClick.AddListener(OnQuit);

        RefreshContinueButton();
    }

    private void RefreshContinueButton()
    {
        if (continueButton == null) return;
        
        // ตรวจสอบว่ามี DataPersistenceManager หรือไม่
        if (DataPersistenceManager.Instance == null)
        {
            Debug.LogWarning("[MainMenuUI] DataPersistenceManager.Instance is null!");
            continueButton.interactable = false;
            return;
        }

        // ตรวจสอบว่ามี save file อยู่จริงหรือไม่
        bool hasSave = false;
        int maxSlots = DataPersistenceManager.Instance.maxSaveSlots;
        
        for (int i = 0; i < maxSlots; i++)
        {
            if (DataPersistenceManager.Instance.SlotExists(i))
            {
                hasSave = true;
                Debug.Log($"[MainMenuUI] Found save file in slot {i}");
                break;
            }
        }
        
        // Continue button จะ enabled ถ้ามี save file เท่านั้น
        continueButton.interactable = hasSave;
        Debug.Log($"[MainMenuUI] Continue button enabled: {hasSave}");
    }

    private void OnNewGame()
    {
        Debug.Log("[MainMenuUI] Starting New Game");
        
        if (DataPersistenceManager.Instance != null)
        {
            DataPersistenceManager.Instance.NewGame();
            Debug.Log("[MainMenuUI] New Game data initialized");
        }
        else
        {
            Debug.LogError("[MainMenuUI] DataPersistenceManager.Instance is null!");
        }
        
        if (!string.IsNullOrEmpty(firstGameSceneName))
        {
            Debug.Log($"[MainMenuUI] Loading scene: {firstGameSceneName}");
            SceneManager.LoadScene(firstGameSceneName);
        }
        else
        {
            Debug.Log("[MainMenuUI] No scene specified for New Game, staying in menu");
        }
    }

    private void OnContinue()
    {
        Debug.Log("[MainMenuUI] Continue button clicked");
        
        // ถ้าไม่มี save file จะไม่ทำงาน (เพราะ button จะ disabled)
        // แต่ใส่ check ไว้เผื่อกรณีที่ถูกเรียกจากที่อื่น
        bool hasSave = false;
        if (DataPersistenceManager.Instance != null)
        {
            int maxSlots = DataPersistenceManager.Instance.maxSaveSlots;
            for (int i = 0; i < maxSlots; i++)
            {
                if (DataPersistenceManager.Instance.SlotExists(i))
                {
                    hasSave = true;
                    break;
                }
            }
        }
        
        if (!hasSave)
        {
            Debug.LogWarning("[MainMenuUI] Continue clicked but no save files found!");
            return;
        }
        
        // เปิดหน้าเลือกช่อง save/load
        if (saveLoadPanel != null)
        {
            Debug.Log("[MainMenuUI] Opening save/load panel in load mode");
            var ui = saveLoadPanel.GetComponent<SaveLoadUI>();
            if (ui != null)
            {
                ui.SetSaveMode(false); // Load mode
                saveLoadPanel.SetActive(true);
            }
            else
            {
                Debug.LogError("[MainMenuUI] SaveLoadUI component not found on saveLoadPanel!");
            }
        }
        else
        {
            Debug.LogError("[MainMenuUI] SaveLoadPanel is not assigned! Please assign SaveLoadUI panel in inspector.");
        }
    }

    private void OnOption()
    {
        Debug.Log("[MainMenuUI] Options button clicked");
        // Optional: open options panel; can add Clear Save Data button there
        
        // ตัวอย่างการเพิ่มปุ่ม Clear Save Data ใน Options
        // คุณสามารถสร้าง Options Panel และเพิ่มปุ่ม Clear Save Data ได้
    }

    private void OnQuit()
    {
        Debug.Log("[MainMenuUI] Quit button clicked");
        
#if UNITY_EDITOR
        Debug.Log("[MainMenuUI] Quitting in Editor mode");
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Debug.Log("[MainMenuUI] Quitting application");
        Application.Quit();
#endif
    }

    /// <summary>
    /// เรียกใช้เมื่อต้องการ refresh หน้า main menu (เช่น กลับมาจาก game scene)
    /// </summary>
    public void RefreshMenu()
    {
        Debug.Log("[MainMenuUI] Refreshing main menu");
        RefreshContinueButton();
    }

    private void Start()
    {
        // Refresh เมื่อเริ่มต้นเพื่อให้แน่ใจว่า Continue button ถูกตั้งค่าอย่างถูกต้อง
        RefreshMenu();
    }
}
