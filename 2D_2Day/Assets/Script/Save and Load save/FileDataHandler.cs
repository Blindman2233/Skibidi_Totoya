using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Handles reading/writing save files as JSON and optional thumbnail images.
/// Path: Application.persistentDataPath / Save Data Json / save_slot_{n}.json and save_slot_{n}_thumb.png
/// </summary>
public class FileDataHandler
{
    private readonly string dataDirPath;
    private readonly string dataFileName = "save_slot_{0}.json";
    private const string ThumbnailFileNameFormat = "save_slot_{0}_thumb.png";

    public FileDataHandler()
    {
        dataDirPath = SaveLoadPathConfig.GetSaveDataDirectoryPath();
    }

    public string GetSlotPath(int slotIndex)
    {
        return Path.Combine(dataDirPath, string.Format(dataFileName, slotIndex));
    }

    public string GetThumbnailPath(int slotIndex)
    {
        return Path.Combine(dataDirPath, string.Format(ThumbnailFileNameFormat, slotIndex));
    }

    public GameData Load(int slotIndex)
    {
        string path = GetSlotPath(slotIndex);
        if (!File.Exists(path))
            return null;

        try
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<GameData>(json);
        }
        catch (Exception e)
        {
            Debug.LogError($"Load save failed: {path}\n{e}");
            return null;
        }
    }

    public void Save(GameData data, int slotIndex, Texture2D thumbnail = null)
    {
        if (data == null) return;

        string path = GetSlotPath(slotIndex);
        data.slotIndex = slotIndex;
        data.saveDate = DateTime.Now.ToString("dd/MM/yyyy");

        try
        {
            Directory.CreateDirectory(dataDirPath);
            string json = JsonUtility.ToJson(data, true); // true = pretty print
            File.WriteAllText(path, json);

            if (thumbnail != null)
            {
                string thumbPath = GetThumbnailPath(slotIndex);
                byte[] bytes = thumbnail.EncodeToPNG();
                File.WriteAllBytes(thumbPath, bytes);
                data.thumbnailFileName = Path.GetFileName(thumbPath);
            }
            else
            {
                data.thumbnailFileName = "";
            }
            
            Debug.Log($"[FileDataHandler] Game saved to slot {slotIndex}: {path}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Save failed: {path}\n{e}");
        }
    }

    /// <summary>
    /// Returns whether the given slot has a save file.
    /// </summary>
    public bool SlotExists(int slotIndex)
    {
        return File.Exists(GetSlotPath(slotIndex));
    }

    /// <summary>
    /// Get save slot metadata without loading full GameData (for UI list). Returns null if slot empty.
    /// </summary>
    public GameData GetSlotInfo(int slotIndex)
    {
        return Load(slotIndex);
    }

    /// <summary>
    /// Delete one slot's save file and thumbnail. For testing / overwrite.
    /// </summary>
    public void DeleteSlot(int slotIndex)
    {
        string path = GetSlotPath(slotIndex);
        if (File.Exists(path))
            File.Delete(path);
        string thumbPath = GetThumbnailPath(slotIndex);
        if (File.Exists(thumbPath))
            File.Delete(thumbPath);
    }

    /// <summary>
    /// Clear all save data (all slots). For testing.
    /// </summary>
    public void ClearAllSaves(int maxSlots = 10)
    {
        for (int i = 0; i < maxSlots; i++)
            DeleteSlot(i);
        Debug.Log("[FileDataHandler] All save data cleared.");
    }
}
