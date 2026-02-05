using System;
using UnityEngine;

namespace DS.Data
{
    using ScriptableObjects;

    [Serializable]
    public class DSDialogueChoiceData
    {
        [field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public DSDialogueSO NextDialogue { get; set; }
        // คะแนน/แต้มความดีที่เปลี่ยนจากการเลือก choice นี้ (อาจเป็นค่าบวกหรือลบก็ได้)
        [field: SerializeField] public int GoodnessPointsDelta { get; set; }
    }
}