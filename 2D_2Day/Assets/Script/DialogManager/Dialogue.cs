using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI; // Required for using the Image component

public class Dialogue : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI textCom;
    public string[] dialogueLines;
    public float textSpeed = 0.05f;

    [Header("Portrait Settings")]
    public Image portraitSlot;       // Drag your UI Image component here
    public Sprite defaultPortrait;   // The image for the main question
    public Sprite correctPortrait;   // Image for the correct response
    public Sprite wrongPortrait;     // Image for the wrong response

    [Header("Choice Settings")]
    public GameObject choicePanel;
    public string[] correctResponse;
    public string[] wrongResponse;

    [Header("Audio (Undertale Style)")]
    public AudioSource audioSource;
    public AudioClip dialogueSound;
    [Range(0.5f, 1.5f)] public float minPitch = 0.9f;
    [Range(0.5f, 1.5f)] public float maxPitch = 1.1f;
    public int soundFrequency = 2;

    [Header("Activation Settings")]
    public GameObject[] activateBefore;
    public GameObject[] activateAfter;

    private int index;
    private bool isChoosing = false;
    private bool isShowingResult = false;
    private WaitForSeconds delay;

    void Start()
    {
        delay = new WaitForSeconds(textSpeed);
        textCom.text = string.Empty;

        // Set the initial portrait
        if (portraitSlot != null && defaultPortrait != null)
        {
            portraitSlot.sprite = defaultPortrait;
        }

        if (choicePanel != null) choicePanel.SetActive(false);
        ToggleGroup(activateBefore, true);
        ToggleGroup(activateAfter, false);

        StartDialogue(dialogueLines);
    }

    void Update()
    {
        if (isChoosing) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (textCom.text == dialogueLines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textCom.text = dialogueLines[index];
                if (audioSource != null) audioSource.Stop();
            }
        }
    }

    public void StartDialogue(string[] newLines)
    {
        if (newLines == null || newLines.Length == 0)
        {
            FinishDialogue();
            return;
        }

        dialogueLines = newLines;
        index = 0;
        isChoosing = false;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        textCom.text = "";
        if (audioSource != null) audioSource.clip = dialogueSound;

        int charCount = 0;
        foreach (char c in dialogueLines[index])
        {
            textCom.text += c;
            charCount++;

            if (audioSource != null && dialogueSound != null && c != ' ' && charCount % soundFrequency == 0)
            {
                audioSource.pitch = Random.Range(minPitch, maxPitch);
                audioSource.Play();
            }
            yield return delay;
        }
    }

    void NextLine()
    {
        if (index < dialogueLines.Length - 1)
        {
            index++;
            StartCoroutine(TypeLine());
        }
        else
        {
            if (choicePanel != null && !isChoosing && !isShowingResult)
            {
                isChoosing = true;
                choicePanel.SetActive(true);
            }
            else
            {
                FinishDialogue();
            }
        }
    }

    public void SelectCorrect()
    {
        isShowingResult = true;
        choicePanel.SetActive(false);

        // Change portrait for the correct answer
        if (portraitSlot != null && correctPortrait != null)
        {
            portraitSlot.sprite = correctPortrait;
        }

        StartDialogue(correctResponse);
    }

    public void SelectWrong()
    {
        isShowingResult = true;
        choicePanel.SetActive(false);

        // Change portrait for the wrong answer
        if (portraitSlot != null && wrongPortrait != null)
        {
            portraitSlot.sprite = wrongPortrait;
        }

        StartDialogue(wrongResponse);
    }

    void FinishDialogue()
    {
        if (isShowingResult && dialogueLines == correctResponse)
        {
            ToggleGroup(activateAfter, true);
        }
        gameObject.SetActive(false);
    }

    void ToggleGroup(GameObject[] group, bool state)
    {
        if (group == null) return;
        foreach (GameObject obj in group)
        {
            if (obj != null) obj.SetActive(state);
        }
    }
}