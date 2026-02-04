using System.Collections.Generic;
using UnityEngine;

public class DialogueActivetor : MonoBehaviour
{
    public List<DialogueLineData> dialogueLines = new List<DialogueLineData>();

    // เมื่อเกิด Event (เช่นเดินไปชน หรือกดคุย) ให้ส่งข้อมูลไปที่ CutsceneSystem
    public void ActivateDialogue()
    {
        CutsceneSystem.Instance.StartCutscene(dialogueLines);
    }
}
