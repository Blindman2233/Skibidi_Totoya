using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton: manages New Game, Save, Load and list of IDataPersistence.
/// Use NewGame() for fresh start, SaveGame(slot) to save, LoadGame(slot) to continue.
/// ClearSaveData() for testing to remove all save files.
/// </summary>
public class DataPersistenceManager : MonoBehaviour
{
    public static DataPersistenceManager Instance { get; private set; }

    [Header("Save slots")]
    [Tooltip("Max number of save slots (e.g. 3 for UI).")]
    public int maxSaveSlots = 3;

    [Header("Thumbnail")]
    [Tooltip("Capture screenshot as save thumbnail when saving.")]
    public bool captureThumbnailOnSave = true;

    [Header("Scene Loading")]
    [Tooltip("Automatically refresh persistence objects after scene load")]
    public bool shouldRefreshAfterSceneLoad = false;

    private List<IDataPersistence> dataPersistenceObjects = new List<IDataPersistence>();
    private FileDataHandler fileDataHandler;
    private GameData currentGameData;
    private bool isNewGame = false; // เพิ่ม flag เพื่อตรวจสอบว่าเป็น New Game หรือไม่

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        fileDataHandler = new FileDataHandler();
        
        // Register for scene load events
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Unregister from scene load events
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnEnable()
    {
        // Auto-find all IDataPersistence in scene (and later from DontDestroyOnLoad)
        RefreshPersistenceObjects();
        
        // ตรวจสอบว่าต้องการ refresh หลังจาก scene load หรือไม่
        if (shouldRefreshAfterSceneLoad)
        {
            Debug.Log("[DataPersistenceManager] Scene loaded, refreshing persistence objects");
            RefreshPersistenceObjects();
            shouldRefreshAfterSceneLoad = false; // Reset flag
        }
    }

    /// <summary>
    /// Called by Unity when a new scene is loaded
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[DataPersistenceManager] Scene '{scene.name}' loaded, mode: {mode}");
        Debug.Log($"[DataPersistenceManager] isNewGame: {isNewGame}");
        
        // Refresh persistence objects หลังจาก scene load เสมอ
        RefreshPersistenceObjects();
        
        // ตรวจสอบว่าเป็น New Game หรือ Load Game
        if (isNewGame)
        {
            Debug.Log("[DataPersistenceManager] New Game detected - keeping original positions");
            // New Game: ไม่โหลดข้อมูล ให้คงค่าเดิมที่วางไว้ใน scene
            isNewGame = false; // Reset flag
            
            foreach (var obj in dataPersistenceObjects)
            {
                if (obj != null)
                {
                    Debug.Log($"[DataPersistenceManager] Notifying {obj.GetType().Name} to keep original position");
                    obj.LoadData(null); // ส่ง null เพื่อให้คงตำแหน่งเดิม
                }
            }
        }
        else if (currentGameData != null)
        {
            Debug.Log("[DataPersistenceManager] Load Game detected - applying saved data");
            // Load Game: โหลดข้อมูลที่บันทึกไว้
            foreach (var obj in dataPersistenceObjects)
            {
                if (obj != null)
                {
                    obj.LoadData(currentGameData);
                }
            }
        }
        else
        {
            Debug.Log("[DataPersistenceManager] No data to apply (first load)");
        }
    }

    /// <summary>
    /// Register a component that implements IDataPersistence. Call from Awake/Start.
    /// </summary>
    public void Register(IDataPersistence persistence)
    {
        if (persistence != null && !dataPersistenceObjects.Contains(persistence))
            dataPersistenceObjects.Add(persistence);
    }

    public void Unregister(IDataPersistence persistence)
    {
        dataPersistenceObjects.Remove(persistence);
    }

    /// <summary>
    /// Find all MonoBehaviours implementing IDataPersistence in the scene.
    /// </summary>
    public void RefreshPersistenceObjects()
    {
        dataPersistenceObjects.Clear();
        var all = FindObjectsOfType<MonoBehaviour>();
        foreach (var mb in all)
        {
            if (mb is IDataPersistence idp)
                Register(idp);
        }
    }

    /// <summary>
    /// Start a new game: create fresh GameData and notify all persistence objects.
    /// </summary>
    public void NewGame()
    {
        currentGameData = new GameData();
        isNewGame = true; // ตั้งค่าว่าเป็น New Game
        Debug.Log("[DataPersistenceManager] Starting New Game with fresh data");
        
        // สำหรับ New Game: ไม่ต้องเรียก LoadData() ทันที
        // ให้ objects คงค่าเดิมที่วางไว้ใน scene ก่อน
        // LoadData() จะถูกเรียกเมื่อจำเป็น (เช่น หลังจาก scene load)
        
        foreach (var obj in dataPersistenceObjects)
        {
            if (obj != null)
            {
                Debug.Log($"[DataPersistenceManager] Notifying {obj.GetType().Name} about New Game");
                // สำหรับ New Game ให้เรียก LoadData แต่ใช้ data เป็น null เพื่อบอกว่าเป็น New Game
                obj.LoadData(null);
            }
        }
    }

    /// <summary>
    /// Save current game to the given slot (0..maxSaveSlots-1). Refreshes data from all IDataPersistence then writes JSON + optional thumbnail.
    /// </summary>
    public void SaveGame(int slotIndex)
    {
        if (currentGameData == null)
            currentGameData = new GameData();

        Debug.Log($"[DataPersistenceManager] Starting save to slot {slotIndex}");
        Debug.Log($"[DataPersistenceManager] Found {dataPersistenceObjects.Count} persistence objects to save");

        foreach (var obj in dataPersistenceObjects)
        {
            if (obj != null)
            {
                Debug.Log($"[DataPersistenceManager] Saving data from: {obj.GetType().Name}");
                obj.SaveData(ref currentGameData);
            }
            else
            {
                Debug.LogWarning("[DataPersistenceManager] Found null persistence object!");
            }
        }

        Texture2D thumb = null;
        if (captureThumbnailOnSave)
            thumb = CaptureThumbnail();

        fileDataHandler.Save(currentGameData, slotIndex, thumb);
        if (thumb != null)
            Destroy(thumb);
        
        Debug.Log($"[DataPersistenceManager] Save completed to slot {slotIndex}");
    }

    /// <summary>
    /// Load game from slot and apply to all IDataPersistence. Returns true if load succeeded.
    /// </summary>
    public bool LoadGame(int slotIndex)
    {
        currentGameData = fileDataHandler.Load(slotIndex);
        isNewGame = false; // ไม่ใช่ New Game
        
        if (currentGameData == null)
            return false;

        RefreshPersistenceObjects();
        foreach (var obj in dataPersistenceObjects)
            obj.LoadData(currentGameData);

        return true;
    }

    /// <summary>
    /// Load the most recent save (highest slot index that exists). Returns true if any save was loaded.
    /// </summary>
    public bool LoadLatestSave()
    {
        for (int i = maxSaveSlots - 1; i >= 0; i--)
        {
            if (fileDataHandler.SlotExists(i))
            {
                return LoadGame(i);
            }
        }
        return false;
    }

    /// <summary>
    /// Get info for UI (date, location, chapter, playtime, thumbnail path) without loading full game. Null if slot empty.
    /// </summary>
    public GameData GetSlotInfo(int slotIndex)
    {
        return fileDataHandler.GetSlotInfo(slotIndex);
    }

    public bool SlotExists(int slotIndex)
    {
        return fileDataHandler.SlotExists(slotIndex);
    }

    /// <summary>
    /// Clear all save data for all slots. Use for testing.
    /// </summary>
    public void ClearSaveData()
    {
        fileDataHandler.ClearAllSaves(maxSaveSlots);
        currentGameData = null;
        Debug.Log("[DataPersistenceManager] Save data cleared. Use New Game or Load to continue.");
    }

    /// <summary>
    /// Optional: get current in-memory GameData (e.g. for UI showing current playtime/location).
    /// </summary>
    public GameData GetCurrentGameData()
    {
        return currentGameData;
    }

    private Texture2D CaptureThumbnail()
    {
        try
        {
            int w = Screen.width;
            int h = Screen.height;
            var rt = new RenderTexture(w, h, 24);
            var cam = Camera.main;
            if (cam == null) return null;
            cam.targetTexture = rt;
            cam.Render();
            RenderTexture.active = rt;
            var tex = new Texture2D(w, h);
            tex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
            tex.Apply();
            cam.targetTexture = null;
            RenderTexture.active = null;
            rt.Release();
            return tex;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Thumbnail capture failed: {e}");
            return null;
        }
    }
}
