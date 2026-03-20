using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Central save data structure. Persisted as JSON per slot.
/// Holds story flags, completed events, playtime, location, chapter and UI display fields.
/// </summary>
[Serializable]
public class GameData
{
    // --- Story / Dialogue System ---
    public List<StoryFlag> storyFlags = new List<StoryFlag>();
    /// <summary> EventData asset names (eventName) that have been completed. Resolved back to EventData at load. </summary>
    public List<string> completedEventNames = new List<string>();

    // --- Display / UI (วันที่, สถานที่, บท, เวลาเล่น) ---
    public string saveDate = "";
    public string currentLocation = "";
    public string chapterTitle = "";
    public int chapterNumber = 1;
    public float playTimeSeconds;

    // --- Player state ---
    public string sceneName = "";
    public float playerPosX;
    public float playerPosY;

    // --- Slot & thumbnail ---
    public int slotIndex;
    /// <summary> Filename of thumbnail image under save folder (e.g. save_slot_0_thumb.png). </summary>
    public string thumbnailFileName = "";

    public GameData()
    {
        saveDate = DateTime.Now.ToString("dd/MM/yyyy");
    }

    /// <summary>
    /// Format playtime as HH:mm:ss for UI.
    /// </summary>
    public string GetPlayTimeFormatted()
    {
        TimeSpan t = TimeSpan.FromSeconds(playTimeSeconds);
        return string.Format("{0:D2}:{1:D2}:{2:D2}", (int)t.TotalHours, t.Minutes, t.Seconds);
    }
}
