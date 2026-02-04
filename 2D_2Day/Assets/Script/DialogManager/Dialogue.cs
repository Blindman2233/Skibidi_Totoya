using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI textCom;
    public string[] dialogueLines;
    public float textSpeed = 0.05f;

    [Header("Choice Settings")]
    public GameObject choicePanel;      // Drag the Panel holding your 2 buttons here
    public string[] correctResponse;    // Dialogue that plays if they pick 'Correct'
    public string[] wrongResponse;      // Dialogue that plays if they pick 'Wrong'

    [Header("Audio (Undertale Style)")]
    public AudioSource audioSource;
    public AudioClip dialogueSound;
    [Range(0.5f, 1.5f)] public float minPitch = 0.9f;
    [Range(0.5f, 1.5f)] public float maxPitch = 1.1f;
    public int soundFrequency = 2;

    [Header("Activation Settings")]
    public GameObject[] activateBefore; // Turns ON at start
    public GameObject[] activateAfter;  // Turns ON ONLY after the correct choice

    private int index;
    private bool isChoosing = false; // Prevents skipping dialogue while buttons are visible
    private WaitForSeconds delay;

    void Start()
    {
        delay = new WaitForSeconds(textSpeed);
        textCom.text = string.Empty;

        // Ensure everything is in the right state at the start
        if (choicePanel != null) choicePanel.SetActive(false);
        ToggleGroup(activateBefore, true);  //
        ToggleGroup(activateAfter, false);  //

        StartDialogue(dialogueLines);
    }

    void Update()
    {
        // Stop player from clicking through if they need to pick a button
        if (isChoosing) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (textCom.text == dialogueLines[index])
            {
                NextLine();
            }
            else
            {
                // Skip typing animation
                StopAllCoroutines();
                textCom.text = dialogueLines[index];
                if (audioSource != null) audioSource.Stop();
            }
        }
    }

    // Now accepts a string array so we can swap between initial lines and responses
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
                // Play() prevents the "machine gun" stacking sound from your video
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
            // If we reach the end of the lines and have a choice panel, show it
            if (choicePanel != null && !isChoosing)
            {
                isChoosing = true;
                choicePanel.SetActive(true);
            }
            else
            {
                // If it wasn't a choice moment, just close the box
                gameObject.SetActive(false); //
            }
        }
    }

    // --- CHOICE BUTTON FUNCTIONS ---
    // Link these to your Button OnClick() events in the Inspector

    public void SelectCorrect()
    {
        choicePanel.SetActive(false);
        ToggleGroup(activateAfter, true); // Success! Turn on the win objects
        StartDialogue(correctResponse);   // Play the "You're right" dialogue
    }

    public void SelectWrong()
    {
        choicePanel.SetActive(false);
        StartDialogue(wrongResponse);     // Play the "Try again" dialogue
    }

    // Helper function to turn lists of objects on or off
    void ToggleGroup(GameObject[] group, bool state)
    {
        if (group == null) return;
        foreach (GameObject obj in group)
        {
            if (obj != null) obj.SetActive(state);
        }
    }
}