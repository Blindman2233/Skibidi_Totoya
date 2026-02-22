using UnityEngine;

public class TestSetupGuide : MonoBehaviour
{
    [Header("Setup Instructions")]
    [TextArea(5, 10)]
    public string setupInstructions = @"
=== ขั้นตอนการตั้งค่าระบบ Dialogue + Story ===

1. สร้าง EventManager GameObject:
   - สร้าง GameObject ใหม่ชื่อ 'EventManager'
   - แนบสคริปต์ EventManager.cs
   - ตั้งค่า All Events ให้มี EventData ทั้งหมด

2. ตั้งค่า DialogueManager UI:
   - เพิ่ม ChoicePanel ใน Dialogue UI
   - สร้าง ChoiceButton Prefab
   - กำหนดใน Inspector ของ DialogueManager

3. สร้าง Test Data:
   - สร้าง DialogueData พร้อม choices
   - สร้าง EventData พร้อม conditions/actions
   - ทดสอบด้วย EventDialogueTrigger

4. ทดสอบการทำงาน:
   - ตั้งค่า Collider บน NPC
   - ตั้งค่า Tag 'Player' บน Player
   - ทดสอบกด E ในระยะ
";

    void Start()
    {
        Debug.Log("=== Test Setup Guide Started ===");
        Debug.Log("Check Inspector for setup instructions");
        
        // ตรวจสอบว่ามี EventManager หรือไม่
        if (EventManager.Instance == null)
        {
            Debug.LogWarning("⚠️ EventManager ยังไม่ได้ตั้งค่าในซีน!");
        }
        else
        {
            Debug.Log("✅ EventManager พร้อมใช้งาน");
        }
        
        // ตรวจสอบ DialogueManager
        if (DialogueManager.Instance == null)
        {
            Debug.LogWarning("⚠️ DialogueManager ยังไม่ได้ตั้งค่าในซีน!");
        }
        else
        {
            Debug.Log("✅ DialogueManager พร้อมใช้งาน");
        }
    }
    
    void Update()
    {
        // ทดสอบดูว่า Player มี Tag ถูกต้องหรือไม่
        if (Input.GetKeyDown(KeyCode.F1))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Debug.Log("✅ Player พร้อมใช้งาน");
            }
            else
            {
                Debug.LogError("❌ ไม่พบ GameObject ที่มี Tag 'Player'");
            }
        }
        
        // ทดสอบดู Story Flags ปัจจุบัน
        if (Input.GetKeyDown(KeyCode.F2))
        {
            if (EventManager.Instance != null)
            {
                Debug.Log("=== Current Story Flags ===");
                foreach (var flag in EventManager.Instance.storyFlags)
                {
                    Debug.Log($"Flag: {flag.flagName} = {flag.intValue} (Active: {flag.isActive})");
                }
            }
        }
        
        // ทดสอบดู Available Events
        if (Input.GetKeyDown(KeyCode.F3))
        {
            if (EventManager.Instance != null)
            {
                var availableEvents = EventManager.Instance.GetAvailableEvents();
                Debug.Log($"=== Available Events: {availableEvents.Count} ===");
                foreach (var evt in availableEvents)
                {
                    Debug.Log($"- {evt.eventName}: {evt.eventDescription}");
                }
            }
        }
    }
}
