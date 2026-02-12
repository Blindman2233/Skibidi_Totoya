using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UnifiedDialogueManager : MonoBehaviour
{
    [System.Serializable]
    public class DialogueLine
    {
        public string characterName;
        public Sprite characterIcon;
        [TextArea(3, 10)]
        public string sentence;
        public AudioClip customSound; // เผื่ออยากให้แต่ละคนเสียงไม่เหมือนกัน
    }
    public static UnifiedDialogueManager Instance;

    [Header("UI References")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Image portraitImage;
    public Animator animator;

    [Header("Choice Settings")]
    public GameObject choicePanel;
    public Button[] choiceButtons; // ลิงก์ปุ่มใน Inspector

    [Header("Audio Settings")]
    public AudioSource audioSource;
    [Range(0.5f, 1.5f)] public float minPitch = 0.9f;
    [Range(0.5f, 1.5f)] public float maxPitch = 1.1f;
    public int soundFrequency = 2;

    [Header("Typing Settings")]
    public float typingSpeed = 0.05f;

    private Queue<DialogueLine> lines;
    private bool isTyping = false;
    private string currentSentence;
    private DialogueLine currentLineData;

    // เก็บค่า Objects ที่จะเปิด/ปิด
    private GameObject[] currentActivateAfter;
    private GameObject[] currentDeactivateAfter;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        lines = new Queue<DialogueLine>();
        dialoguePanel.SetActive(false);
        if (choicePanel != null) choicePanel.SetActive(false);
    }

    public void StartDialogue(List<DialogueLine> dialogueGroup, GameObject[] activateBefore = null, GameObject[] deactivateBefore = null, GameObject[] activateAfter = null, GameObject[] deactivateAfter = null)
    {
        // 1. จัดการ Objects (จาก Dialogue.cs)
        ToggleGroup(activateBefore, true);
        ToggleGroup(deactivateBefore, false);
        currentActivateAfter = activateAfter;
        currentDeactivateAfter = deactivateAfter;

        // 2. ตั้งค่า UI
        dialoguePanel.SetActive(true);
        if (animator != null) animator.Play("show");

        lines.Clear();
        foreach (DialogueLine line in dialogueGroup)
        {
            lines.Enqueue(line);
        }

        DisplayNextLine();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && dialoguePanel.activeSelf)
        {
            if (isTyping)
            {
                StopAllCoroutines();
                dialogueText.text = currentSentence;
                isTyping = false;
                if (audioSource != null) audioSource.Stop();
            }
            else if (choicePanel != null && !choicePanel.activeSelf)
            {
                DisplayNextLine();
            }
        }
    }

    public void DisplayNextLine()
    {
        if (lines.Count == 0)
        {
            ShowChoicesOrEnd();
            return;
        }

        currentLineData = lines.Dequeue();
        currentSentence = currentLineData.sentence;
        nameText.text = currentLineData.characterName;
        portraitImage.sprite = currentLineData.characterIcon;

        StopAllCoroutines();
        StartCoroutine(TypeSentence());
    }

    IEnumerator TypeSentence()
    {
        isTyping = true;
        dialogueText.text = "";
        int charCount = 0;

        foreach (char letter in currentSentence.ToCharArray())
        {
            dialogueText.text += letter;
            charCount++;

            // เสียงแบบ Undertale (จาก Dialogue.cs)
            if (audioSource != null && charCount % soundFrequency == 0)
            {
                audioSource.pitch = Random.Range(minPitch, maxPitch);
                audioSource.PlayOneShot(currentLineData.customSound != null ? currentLineData.customSound : audioSource.clip);
            }

            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    void ShowChoicesOrEnd()
    {
        // ถ้ามี Choice ให้แสดง Choice ถ้าไม่มีให้จบเลย
        if (choicePanel != null && choiceButtons.Length > 0)
        {
            choicePanel.SetActive(true);
        }
        else
        {
            EndDialogue();
        }
    }

    public void EndDialogue()
    {
        if (animator != null) animator.Play("hide");
        dialoguePanel.SetActive(false);
        if (choicePanel != null) choicePanel.SetActive(false);

        ToggleGroup(currentActivateAfter, true);
        ToggleGroup(currentDeactivateAfter, false);
    }

    void ToggleGroup(GameObject[] group, bool state)
    {
        if (group == null) return;
        foreach (GameObject obj in group) if (obj != null) obj.SetActive(state);
    }
}