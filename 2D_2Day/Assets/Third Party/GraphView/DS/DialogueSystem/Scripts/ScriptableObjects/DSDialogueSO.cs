using System.Collections.Generic;
using UnityEngine;

namespace DS.ScriptableObjects
{
    using Data;
    using Enumerations;

    public class DSDialogueSO : ScriptableObject
    {
        [field: SerializeField] public string DialogueName { get; set; }
        [field: SerializeField] [field: TextArea()] public string Text { get; set; }
        [field: SerializeField] public List<DSDialogueChoiceData> Choices { get; set; }
        [field: SerializeField] public DSDialogueType DialogueType { get; set; }
        [field: SerializeField] public bool IsStartingDialogue { get; set; }
        // เสียงพูดของบทนี้
        [field: SerializeField] public AudioClip VoiceClip { get; set; }
        // รูป/หน้า Character สำหรับบทนี้ (ใช้เปลี่ยนอารมณ์ใบหน้าได้)
        [field: SerializeField] public Sprite CharacterArt { get; set; }

        public void Initialize(string dialogueName, string text, List<DSDialogueChoiceData> choices, DSDialogueType dialogueType, bool isStartingDialogue, AudioClip voiceClip, Sprite characterArt)
        {
            DialogueName = dialogueName;
            Text = text;
            Choices = choices;
            DialogueType = dialogueType;
            IsStartingDialogue = isStartingDialogue;
            VoiceClip = voiceClip;
            CharacterArt = characterArt;
        }
    }
}