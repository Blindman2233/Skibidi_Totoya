using System.Collections;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI textCom;
    public string[] dialogueLines; // Your initial question
    public float textSpeed = 0.05f;

    [Header("Choice Settings")]
    public GameObject choicePanel;      // The container for your 2 buttons
    public string[] correctResponse;    // Dialogue if they pick 'Correct'
    public string[] wrongResponse;      // Dialogue if they pick 'Wrong'

    [Header("Audio (Undertale Style)")]
    public AudioSource audioSource;
    public AudioClip dialogueSound;
    [Range(0.5f, 1.5f)] public float minPitch = 0.9f;
    [Range(0.5f, 1.5f)] public float maxPitch = 1.1f;
    public int soundFrequency = 2;

    [Header("Activation Settings")]
    public GameObject[] activateBefore; // Turns ON at start
    public GameObject[] activateAfter;  // Turns ON ONLY after the correct response ends

    private int index;
    private bool isChoosing = false;      // Stops clicking while buttons are active
    private bool isShowingResult = false;  // Tracks if we are playing response lines
    private WaitForSeconds delay;

    void Start()
    {
        delay = new WaitForSeconds(textSpeed);
        textCom.text = string.Empty;

        // Setup initial states
        if (choicePanel != null) choicePanel.SetActive(false);
        ToggleGroup(activateBefore, true);
        ToggleGroup(activateAfter, false);

        StartDialogue(dialogueLines);
    }

    void Update()
    {
        // Don't skip text if the player needs to pick a button
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
                audioSource.Play(); // Stops audio stacking from your video
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
            // If the question is over, show the buttons
            if (choicePanel != null && !isChoosing && !isShowingResult)
            {
                isChoosing = true;
                choicePanel.SetActive(true);
            }
            else
            {
                // If the response dialogue is finished, turn on win objects
                if (isShowingResult) ToggleGroup(activateAfter, true);
                gameObject.SetActive(false);
            }
        }
    }

    // --- BUTTON FUNCTIONS ---
    // Link these to your Buttons' OnClick() events in the Inspector

    public void SelectCorrect()
    {
        isShowingResult = true;
        choicePanel.SetActive(false);
        StartDialogue(correctResponse); // Runs the correct response dialogue
    }

    public void SelectWrong()
    {
        isShowingResult = true;
        choicePanel.SetActive(false);
        StartDialogue(wrongResponse); // Runs the wrong response dialogue
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