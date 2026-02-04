using UnityEngine;

[System.Serializable]
public class DialogueLineData
{
    public string characterName;      // ชื่อตัวละคร
    [TextArea(3, 10)]
    public string text;               // บทพูด
    public Sprite characterPortrait;  // รูปสีหน้า (เปลี่ยนตามบรรทัดนี้)

    // สำหรับ Dialogue Tree
    public bool hasChoices;           // บรรทัดนี้มีตัวเลือกไหม?
    public DialogueChoice[] choices;  // รายการตัวเลือก
}

[System.Serializable]
public class DialogueChoice
{
    public string choiceText;         // ข้อความบนปุ่ม
    public int nextLineIndex;         // กดแล้วให้กระโดดไปที่ Dialogue Line ลำดับที่เท่าไหร่
}
