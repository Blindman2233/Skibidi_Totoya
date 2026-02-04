using UnityEngine;


public class InteractNPC : MonoBehaviour
{

    [Header("UI Settings")]
    public GameObject promptObject; // ไอคอนปุ่ม "E"
    public GameObject text;         // ข้อความเช่น "Press E to Talk"

    private DialogueActivetor dialogueActivator;
    private bool isPlayerInRange = false;

    private void Awake()
    {
        // ดึง Component DialogueActivator ที่อยู่ใน Object เดียวกันมาเตรียมไว้
        dialogueActivator = GetComponent<DialogueActivetor>();
    }

    private void Update()
    {
        // ถ้าผู้เล่นอยู่ในระยะ และกดปุ่ม E
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (dialogueActivator != null)
            {
                // สั่งให้บทสนทนาเริ่มทำงาน
                dialogueActivator.ActivateDialogue();
                Debug.Log("OK fine");
                // (Option) ซ่อนปุ่ม E ทันทีที่เริ่มคุยเพื่อไม่ให้เกะกะสายตา
                HidePrompt();
            }
            else
            {
                Debug.LogError("หา DialogueActivetor ใน NPC ตัวนี้ไม่เจอ!");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            ShowPrompt();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            HidePrompt();
        }
    }

    private void ShowPrompt()
    {
        if (promptObject != null) promptObject.SetActive(true);
        if (text != null) text.SetActive(true);
    }

    private void HidePrompt()
    {
        if (promptObject != null) promptObject.SetActive(false);
        if (text != null) text.SetActive(false);
    }
}