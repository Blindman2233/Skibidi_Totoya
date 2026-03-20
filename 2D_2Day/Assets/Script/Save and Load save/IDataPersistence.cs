using UnityEngine;

/// <summary>
/// Interface for any component that needs to save/load its data via DataPersistenceManager.
/// Register in Awake/Start and implement LoadData / SaveData.
/// </summary>
public interface IDataPersistence
{
    void LoadData(GameData data);
    void SaveData(ref GameData data);
}
