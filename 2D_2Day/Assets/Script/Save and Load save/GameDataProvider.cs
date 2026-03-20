using UnityEngine;

/// <summary>
/// Holds current location name and chapter title for save/load UI.
/// Implements IDataPersistence so these values are stored in GameData.
/// Set currentLocation and chapterTitle from your level/trigger scripts as needed.
/// </summary>
public class GameDataProvider : MonoBehaviour, IDataPersistence
{
    public static GameDataProvider Instance { get; private set; }

    [Header("Current state (can be set by code or inspector)")]
    public string currentLocation = "";
    public string chapterTitle = "";
    public int chapterNumber = 1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        if (DataPersistenceManager.Instance != null)
            DataPersistenceManager.Instance.Register(this);
    }

    private void OnDestroy()
    {
        if (DataPersistenceManager.Instance != null)
            DataPersistenceManager.Instance.Unregister(this);
        if (Instance == this)
            Instance = null;
    }

    public void LoadData(GameData data)
    {
        if (data == null) return;
        currentLocation = data.currentLocation ?? "";
        chapterTitle = data.chapterTitle ?? "";
        chapterNumber = data.chapterNumber;
    }

    public void SaveData(ref GameData data)
    {
        if (data == null) return;
        data.currentLocation = currentLocation;
        data.chapterTitle = chapterTitle;
        data.chapterNumber = chapterNumber;
    }

    public void SetLocation(string location)
    {
        currentLocation = location;
    }

    public void SetChapter(string title, int number)
    {
        chapterTitle = title;
        chapterNumber = number;
    }
}
