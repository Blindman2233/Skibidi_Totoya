using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// UI for Save/Load screen: shows multiple slots (thumbnail, date, location, chapter, playtime),
/// Back, Save และ Load buttons. เลือกช่องแล้วกด Save หรือ Load ได้
/// </summary>
public class SaveLoadUI : MonoBehaviour
{
    [Header("Slot UI (assign 3 or more)")]
    public List<SaveSlotUI> slots = new List<SaveSlotUI>();

    [Header("Buttons")]
    public Button backButton;
    public Button saveButton;
    public Button loadButton;

    [Header("Mode")]
    [Tooltip("True = หน้า Save (เน้นบันทึก). False = หน้า Load/Continue (เน้นโหลด).")]
    public bool isSaveMode = true;

    [Header("Scenes")]
    [Tooltip("Default game scene to load if save data doesn't specify scene name")]
    public string defaultGameSceneName = "SampleScene";

    private int selectedSlotIndex = -1;

    private void OnEnable()
    {
        RefreshAllSlots();
        selectedSlotIndex = -1;
        
        // ตรวจสอบว่ามีการกำหนด buttons หรือไม่
        if (backButton != null) 
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(OnBack);
        }
        else
        {
            Debug.LogWarning("[SaveLoadUI] Back button not assigned!");
        }
        
        if (saveButton != null) 
        {
            saveButton.onClick.RemoveAllListeners();
            saveButton.onClick.AddListener(OnSave);
        }
        else
        {
            Debug.LogWarning("[SaveLoadUI] Save button not assigned!");
        }
        
        if (loadButton != null) 
        {
            loadButton.onClick.RemoveAllListeners();
            loadButton.onClick.AddListener(OnLoad);
        }
        else
        {
            Debug.LogWarning("[SaveLoadUI] Load button not assigned!");
        }
        
        UpdateLoadButtonState();
        UpdateSaveButtonState();
    }

    public void SetSaveMode(bool saveMode)
    {
        isSaveMode = saveMode;
    }

    public void RefreshAllSlots()
    {
        if (DataPersistenceManager.Instance == null) return;
        int max = DataPersistenceManager.Instance.maxSaveSlots;
        for (int i = 0; i < slots.Count && i < max; i++)
        {
            var slot = slots[i];
            if (slot == null) continue;
            GameData info = DataPersistenceManager.Instance.GetSlotInfo(i);
            slot.SetData(i, info, this);
            int index = i;
            if (slot.selectButton != null)
            {
                slot.selectButton.onClick.RemoveAllListeners();
                slot.selectButton.onClick.AddListener(() => OnSlotSelected(index));
            }
        }
    }

    private void OnSlotSelected(int index)
    {
        selectedSlotIndex = index;
        Debug.Log($"[SaveLoadUI] Slot {index} selected. Mode: {(isSaveMode ? "Save" : "Load")}");
        UpdateLoadButtonState();
        UpdateSaveButtonState();
        
        // ใน Load mode ไม่ต้อง auto-load ให้รอให้ผู้เล่นกด Load button ก่อน
        // Auto-load ถูกลบออกไปแล้ว
    }

    private void UpdateLoadButtonState()
    {
        if (loadButton == null) return;
        bool hasSlot = selectedSlotIndex >= 0 && DataPersistenceManager.Instance != null && DataPersistenceManager.Instance.SlotExists(selectedSlotIndex);
        loadButton.interactable = hasSlot;
    }

    private void UpdateSaveButtonState()
    {
        if (saveButton == null) return;
        saveButton.interactable = selectedSlotIndex >= 0;
    }

    private void OnSave()
    {
        if (selectedSlotIndex < 0)
        {
            Debug.LogWarning("[SaveLoadUI] No slot selected for save!");
            return;
        }
        Debug.Log($"[SaveLoadUI] Saving to slot {selectedSlotIndex}");
        if (DataPersistenceManager.Instance != null)
        {
            DataPersistenceManager.Instance.SaveGame(selectedSlotIndex);
            RefreshAllSlots();
        }
    }

    private void OnLoad()
    {
        if (selectedSlotIndex < 0)
        {
            Debug.LogWarning("[SaveLoadUI] No slot selected for load!");
            return;
        }
        if (DataPersistenceManager.Instance != null && DataPersistenceManager.Instance.SlotExists(selectedSlotIndex))
        {
            Debug.Log($"[SaveLoadUI] Loading from slot {selectedSlotIndex}");
            
            // โหลดข้อมูลเกม
            bool loadSuccess = DataPersistenceManager.Instance.LoadGame(selectedSlotIndex);
            
            if (loadSuccess)
            {
                Debug.Log("[SaveLoadUI] Load successful, switching to game scene");
                
                // หาชื่อ scene จากข้อมูลที่โหลด
                string sceneToLoad = null;
                GameData loadedData = DataPersistenceManager.Instance.GetCurrentGameData();
                
                if (loadedData != null && !string.IsNullOrEmpty(loadedData.sceneName))
                {
                    sceneToLoad = loadedData.sceneName;
                    Debug.Log($"[SaveLoadUI] Loading saved scene: {sceneToLoad}");
                }
                else
                {
                    sceneToLoad = defaultGameSceneName;
                    Debug.Log($"[SaveLoadUI] No scene in save data, using default: {sceneToLoad}");
                }
                
                // โหลด scene และตั้งค่าให้ refresh หลังโหลด
                if (!string.IsNullOrEmpty(sceneToLoad))
                {
                    // บอก DataPersistenceManager ให้ refresh หลังโหลด scene
                    DataPersistenceManager.Instance.shouldRefreshAfterSceneLoad = true;
                    SceneManager.LoadScene(sceneToLoad);
                }
                else
                {
                    Debug.LogError("[SaveLoadUI] No scene to load!");
                }
            }
            else
            {
                Debug.LogError("[SaveLoadUI] Failed to load game data");
            }
        }
        else
        {
            Debug.LogWarning($"[SaveLoadUI] No save data in slot {selectedSlotIndex}");
        }
    }

    private void OnBack()
    {
        gameObject.SetActive(false);
    }

    public int GetSelectedSlotIndex()
    {
        return selectedSlotIndex;
    }
}
