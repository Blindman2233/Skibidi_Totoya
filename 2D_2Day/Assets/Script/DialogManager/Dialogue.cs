using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueConfig : MonoBehaviour
{
    [System.Serializable]
    public class ActorMovement
    {
        public string character;
        public GameObject actorGameObject;
        public Vector2 actorStartPosition;
        public Vector2 actorEndPosition;
        public bool leaveScene;
    }

    [System.Serializable]
    public class DialogueChoice
    {
        public string choiceText;
        public GameObject nextDialogueObject; // เชื่อมไปยัง Script Dialogue ตัวถัดไป
    }

    [System.Serializable]
    public class DialogueLine
    {
        [Header("Text Event")]
        public bool isTextEvent = true;
        [TextArea(3, 5)] public string lineText;
        public string characterToShow;
        public string characterExpression;

        // --- ย้ายเสียงมาไว้ในแต่ละบรรทัดที่นี่ ---
        public AudioClip dialogueSound;

        [Header("Movement Event")]
        public bool isMovementEvent;
        public List<ActorMovement> actorsInScene;

        [Header("Choices")]
        public bool hasChoices;
        public List<DialogueChoice> choices;
    }

    [Header("Main Settings")]
    public List<DialogueLine> dialogueLines;
    public TextMeshProUGUI textDisplay;
    public float textSpeed = 0.05f;

    [Header("Audio Config")]
    public AudioSource audioSource;
    [Range(0.5f, 1.5f)] public float minPitch = 0.9f;
    [Range(0.5f, 1.5f)] public float maxPitch = 1.1f;
    public int soundFrequency = 2; // เล่นเสียงทุกๆ X ตัวอักษร

    [Header("UI Components")]
    public GameObject choicePanel;
    public GameObject choiceButtonPrefab;
    public GameObject[] objectsToEnableAtStart;
    public GameObject[] objectsToEnableAtEnd;

    private int index;
    private bool isTyping;

    void Start()
    {
        textDisplay.text = string.Empty;
        if (choicePanel != null) choicePanel.SetActive(false);

        foreach (GameObject obj in objectsToEnableAtStart) if (obj) obj.SetActive(true);
        foreach (GameObject obj in objectsToEnableAtEnd) if (obj) obj.SetActive(false);

        StartDialogue();
    }

    void Update()
    {
        // คลิกเพื่อข้ามการพิมพ์ หรือไปประโยคถัดไป (ยกเว้นตอนมี Choice ขึ้นมา)
        if (Input.GetMouseButtonDown(0) && (choicePanel == null || !choicePanel.activeSelf))
        {
            if (textDisplay.text == dialogueLines[index].lineText)
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textDisplay.text = dialogueLines[index].lineText;
                isTyping = false;
            }
        }
    }

    void StartDialogue()
    {
        index = 0;
        DisplayLine();
    }

    void DisplayLine()
    {
        DialogueLine current = dialogueLines[index];

        // 1. จัดการเรื่องการเคลื่อนที่
        if (current.isMovementEvent)
        {
            foreach (var actor in current.actorsInScene)
            {
                if (actor.actorGameObject != null)
                {
                    StartCoroutine(AnimateActor(actor));
                }
            }
        }

        // 2. เริ่มพิมพ์ข้อความ
        if (current.isTextEvent)
        {
            StartCoroutine(TypeEffect());
        }
    }

    IEnumerator AnimateActor(ActorMovement actor)
    {
        float progress = 0;
        actor.actorGameObject.transform.localPosition = actor.actorStartPosition;

        while (progress < 1f)
        {
            progress += Time.deltaTime * 2f; // ความเร็วการเคลื่อนที่
            actor.actorGameObject.transform.localPosition = Vector2.Lerp(actor.actorStartPosition, actor.actorEndPosition, progress);
            yield return null;
        }

        if (actor.leaveScene) actor.actorGameObject.SetActive(false);
    }

    IEnumerator TypeEffect()
    {
        isTyping = true;
        textDisplay.text = string.Empty;
        int charCount = 0;

        // ดึงไฟล์เสียงของบรรทัดปัจจุบัน
        AudioClip currentClip = dialogueLines[index].dialogueSound;

        foreach (char letter in dialogueLines[index].lineText.ToCharArray())
        {
            textDisplay.text += letter;
            charCount++;

            // เล่นเสียงเฉพาะตัวของบรรทัดนี้
            if (audioSource != null && currentClip != null && letter != ' ' && charCount % soundFrequency == 0)
            {
                audioSource.pitch = Random.Range(minPitch, maxPitch);
                audioSource.PlayOneShot(currentClip);
            }
            yield return new WaitForSeconds(textSpeed);
        }
        isTyping = false;

        // ถ้าพิมพ์จบแล้วมี Choice ให้แสดงทันที
        if (dialogueLines[index].hasChoices)
        {
            Invoke("ShowChoiceUI", 0.2f);
        }
    }

    void ShowChoiceUI()
    {
        if (choicePanel == null) return;

        choicePanel.SetActive(true);
        foreach (Transform child in choicePanel.transform) Destroy(child.gameObject);

        foreach (var choice in dialogueLines[index].choices)
        {
            GameObject btn = Instantiate(choiceButtonPrefab, choicePanel.transform);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = choice.choiceText;
            btn.GetComponent<Button>().onClick.AddListener(() => OnChoiceSelected(choice.nextDialogueObject));
        }
    }

    void OnChoiceSelected(GameObject nextTarget)
    {
        if (nextTarget != null)
        {
            nextTarget.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }

    void NextLine()
    {
        if (index < dialogueLines.Count - 1)
        {
            index++;
            DisplayLine();
        }
        else if (!dialogueLines[index].hasChoices)
        {
            EndConversation();
        }
    }

    void EndConversation()
    {
        foreach (GameObject obj in objectsToEnableAtEnd) if (obj) obj.SetActive(true);
        this.gameObject.SetActive(false);
    }
}