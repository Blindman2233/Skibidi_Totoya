using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

/// <summary>
/// One save slot row: thumbnail, date, location, chapter + playtime. Assign in inspector.
/// </summary>
public class SaveSlotUI : MonoBehaviour
{
    [Header("References")]
    public RawImage thumbnailImage;
    public TextMeshProUGUI dateLocationText;
    public TextMeshProUGUI chapterPlayTimeText;
    public Button selectButton;

    [Header("Empty slot text")]
    public string emptySlotDateLocation = "---";
    public string emptySlotChapterTime = "---";

    private int slotIndex;
    private SaveLoadUI parentUI;

    public void SetData(int index, GameData data, SaveLoadUI parent)
    {
        slotIndex = index;
        parentUI = parent;

        if (data == null)
        {
            if (dateLocationText != null) dateLocationText.text = emptySlotDateLocation;
            if (chapterPlayTimeText != null) chapterPlayTimeText.text = emptySlotChapterTime;
            if (thumbnailImage != null) thumbnailImage.texture = null;
            return;
        }

        string dateLoc = data.saveDate + " " + data.currentLocation;
        if (dateLocationText != null) dateLocationText.text = dateLoc;
        string chTime = "CH." + data.chapterNumber + " " + data.chapterTitle + " " + data.GetPlayTimeFormatted();
        if (chapterPlayTimeText != null) chapterPlayTimeText.text = chTime;

        if (thumbnailImage != null)
        {
            if (thumbnailImage.texture is Texture2D prev)
                Destroy(prev);
            Texture2D thumb = LoadThumbnail(data);
            thumbnailImage.texture = thumb;
        }
    }

    private Texture2D LoadThumbnail(GameData data)
    {
        if (string.IsNullOrEmpty(data.thumbnailFileName)) return null;
        string path = Path.Combine(SaveLoadPathConfig.GetSaveDataDirectoryPath(), data.thumbnailFileName);
        if (!File.Exists(path)) return null;
        try
        {
            byte[] bytes = File.ReadAllBytes(path);
            var tex = new Texture2D(2, 2);
            if (tex.LoadImage(bytes)) return tex;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Load thumbnail failed: {e}");
        }
        return null;
    }
}
