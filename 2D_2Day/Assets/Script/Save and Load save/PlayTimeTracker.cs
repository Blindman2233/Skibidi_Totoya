using UnityEngine;

/// <summary>
/// Tracks total play time and persists via IDataPersistence.
/// Attach to a persistent object (e.g. same as DataPersistenceManager or EventManager).
/// </summary>
public class PlayTimeTracker : MonoBehaviour, IDataPersistence
{
    public static PlayTimeTracker Instance { get; private set; }

    private float totalPlayTimeSeconds;

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

    private void Update()
    {
        totalPlayTimeSeconds += Time.deltaTime;
    }

    public void LoadData(GameData data)
    {
        if (data == null) return;
        totalPlayTimeSeconds = data.playTimeSeconds;
    }

    public void SaveData(ref GameData data)
    {
        if (data == null) return;
        data.playTimeSeconds = totalPlayTimeSeconds;
    }

    public float GetTotalPlayTimeSeconds()
    {
        return totalPlayTimeSeconds;
    }
}
