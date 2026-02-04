using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class CutsceneSystem : MonoBehaviour
{
    public static CutsceneSystem Instance;

    [Header("UI References")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Image portraitImage;
    public GameObject dialoguePanel;
    public GameObject choiceParent;
    public Button choiceButtonPrefab;

    [Header("Typing & Audio Settings")]
    public float textSpeed = 0.05f;
    public AudioSource audioSource;
    public AudioClip dialogueSound;
    [Range(0.5f, 1.5f)] public float minPitch = 0.9f;
    [Range(0.5f, 1.5f)] public float maxPitch = 1.1f;
    public int soundFrequency = 2;

    private List<DialogueLineData> currentLines;
    private int currentIndex = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    private void Awake() => Instance = this;

    public void StartCutscene(List<DialogueLineData> lines)
    {
        currentLines = lines;
        currentIndex = 0;
        dialoguePanel.SetActive(true);
        DisplayLineData();
    }

    public void DisplayLineData()
    {
        if (currentIndex < currentLines.Count)
        {
            DialogueLineData line = currentLines[currentIndex];

            // อัปเดตชื่อและรูปทันที
            nameText.text = line.characterName;
            portraitImage.sprite = line.characterPortrait;

            // เริ่มการพิมพ์ข้อความแบบ Typewriter
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeLine(line.text));
        }
        else
        {
            EndCutscene();
        }
    }

    IEnumerator TypeLine(string fullText)
    {
        isTyping = true;
        dialogueText.text = "";
        int charCount = 0;

        foreach (char c in fullText)
        {
            dialogueText.text += c;
            charCount++;

            // เล่นเสียงแบบ Undertale
            if (audioSource != null && dialogueSound != null && c != ' ' && charCount % soundFrequency == 0)
            {
                audioSource.pitch = Random.Range(minPitch, maxPitch);
                audioSource.PlayOneShot(dialogueSound);
            }

            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;
        CheckForChoices(); // เมื่อพิมพ์เสร็จค่อยเช็กว่ามีตัวเลือกไหม
    }

    private void CheckForChoices()
    {
        DialogueLineData line = currentLines[currentIndex];
        if (line.hasChoices)
        {
            ShowChoices(line.choices);
        }
    }

    public void OnClickNext()
    {
        if (currentLines == null || currentIndex >= currentLines.Count) return;

        // ถ้ากำลังพิมพ์อยู่ ให้กดเพื่อแสดงข้อความทั้งหมดทันที (Skip typing)
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.text = currentLines[currentIndex].text;
            isTyping = false;
            CheckForChoices();
            return;
        }

        // ถ้ามีตัวเลือกค้างอยู่ ห้ามกด Next
        if (currentLines[currentIndex].hasChoices) return;

        currentIndex++;
        DisplayLineData();
    }

    private void ShowChoices(DialogueChoice[] choices)
    {
        ClearChoices();
        choiceParent.SetActive(true);

        foreach (var choice in choices)
        {
            DialogueChoice tempChoice = choice;
            Button btn = Instantiate(choiceButtonPrefab, choiceParent.transform);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = tempChoice.choiceText;

            btn.onClick.AddListener(() => {
                currentIndex = tempChoice.nextLineIndex;
                choiceParent.SetActive(false);
                ClearChoices();
                DisplayLineData();
            });
        }
    }

    private void ClearChoices()
    {
        foreach (Transform child in choiceParent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void EndCutscene()
    {
        dialoguePanel.SetActive(false);
    }
}