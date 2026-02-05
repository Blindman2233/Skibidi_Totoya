using System;
using UnityEngine;

namespace DS.Data.Save
{
    [Serializable]
    public class DSChoiceSaveData
    {
        [field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public string NodeID { get; set; }
        // คะแนน/แต้มความดีที่เปลี่ยนจากการเลือก choice นี้ (อาจเป็นค่าบวกหรือลบก็ได้)
        [field: SerializeField] public int GoodnessPointsDelta { get; set; }
    }
}