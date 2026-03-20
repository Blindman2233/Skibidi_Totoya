using System.IO;
using UnityEngine;

/// <summary>
/// ชื่อโฟลเดอร์ที่ใช้เก็บไฟล์ Save (JSON + thumbnail) ใต้ persistentDataPath.
/// ใช้ร่วมกับ FileDataHandler และ Editor tool Clear Save Data
/// </summary>
public static class SaveLoadPathConfig
{
    public const string SaveDataFolderName = "Save Data Json";

    public static string GetSaveDataDirectoryPath()
    {
        return Path.Combine(Application.persistentDataPath, SaveDataFolderName);
    }
}
