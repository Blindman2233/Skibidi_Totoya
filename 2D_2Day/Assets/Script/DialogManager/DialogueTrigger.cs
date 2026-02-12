using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueCharacter
{
    public string name;
    public Sprite icon;
}

[System.Serializable]
public class DialogueLine
{
    public DialogueCharacter character;
    [TextArea(3, 10)]
    public string line;
    internal object customSound;
}

[System.Serializable]
public class DialogueText
{
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();
}

public class DialogueTrigger : MonoBehaviour
{
    public DialogueText dialogue;
    public GameObject DialogueUI;

    public void TriggerDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogue);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            DialogueUI.SetActive(true);
            StartCoroutine(WaitForSeconds(1f));
            TriggerDialogue();
        }
    }

    IEnumerator WaitForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}