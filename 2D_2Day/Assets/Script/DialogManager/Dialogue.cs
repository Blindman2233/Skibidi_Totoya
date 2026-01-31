using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textCom;
    public float textSpeed;
    public string[] dialogueLines;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip dialogueSound;

    [Range(0.5f, 1.5f)]
    public float minPitch = 0.9f;
    [Range(0.5f, 1.5f)]
    public float maxPitch = 1.1f;

    [Tooltip("Play sound every X characters. 1 = Every letter, 2 = Every other letter.")]
    public int soundFrequency = 2; // Set to 2 for a cleaner "Undertale" sound

    private int index;

    void Start()
    {
        textCom.text = string.Empty;
        StartDialogue();
    }

    void Update()
    {
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
                // Optional: Play one final beep when skipping to end
                if (audioSource != null) audioSource.Stop();
            }
        }
    }

    void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        textCom.text = string.Empty;

        // Assign the clip ONCE at the start so we can use Play() safely
        if (audioSource != null && dialogueSound != null)
        {
            audioSource.clip = dialogueSound;
        }

        int charCount = 0; // To track when to beep

        foreach (char c in dialogueLines[index].ToCharArray())
        {
            textCom.text += c;
            charCount++;

            // --- FIXED SOUND LOGIC ---
            if (audioSource != null && dialogueSound != null)
            {
                // 1. Don't beep for spaces
                // 2. Only beep based on frequency (e.g., every 2nd letter)
                if (c != ' ' && charCount % soundFrequency == 0)
                {
                    audioSource.pitch = Random.Range(minPitch, maxPitch);

                    // THE FIX: Play() restarts the sound, cutting off the old one.
                    // This stops the volume from exploding.
                    audioSource.Play();
                }
            }
            // -----------------------------

            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (index < dialogueLines.Length - 1)
        {
            index++;
            textCom.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}