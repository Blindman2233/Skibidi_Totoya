using System.Collections.Generic;
using UnityEngine;

public class DialogueActivetor : MonoBehaviour
{
    public List<DialogueLineData> dialogueLines = new List<DialogueLineData>();

    // ������Դ Event (���Թ仪� ���͡����) ����觢�����价�� CutsceneSystem
    public void ActivateDialogue()
    {
        CutsceneSystem.Instance.StartCutscene(dialogueLines);
    }
    //Test Cursor Help Code in this line
    //Test Windsurf AI
}
