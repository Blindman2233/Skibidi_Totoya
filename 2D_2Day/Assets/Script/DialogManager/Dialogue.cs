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
    public int soundFrequency = 2;

    [Header("After Dialogue Settings")]
    public GameObject uiToActivate;      // Drag your Win Screen/Button here

    // CHANGED: This is now an Array [] so you can add multiple objects (Player, Door, Key, etc.)
    public GameObject[] objectsToActivate;

    private int index;

    void Start()
    {
        textCom.text = string.Empty;

        // 1. Hide the UI at start
        if (uiToActivate != null) uiToActivate.SetActive(false);

        // 2. Hide ALL objects in the list at start
        if (objectsToActivate != null)
        {
            foreach (GameObject obj in objectsToActivate)
            {
                if (obj != null) obj.SetActive(false);
            }
        }

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

        if (audioSource != null && dialogueSound != null)
        {
            audioSource.clip = dialogueSound;
        }

        int charCount = 0;

        foreach (char c in dialogueLines[index].ToCharArray())
        {
            textCom.text += c;
            charCount++;

            if (audioSource != null && dialogueSound != null)
            {
                if (c != ' ' && charCount % soundFrequency == 0)
                {
                    audioSource.pitch = Random.Range(minPitch, maxPitch);
                    audioSource.Play();
                }
            }

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
            // --- DIALOGUE IS DONE! ---

            // 1. Turn on the UI
            if (uiToActivate != null) uiToActivate.SetActive(true);

            // 2. Turn on ALL objects in the list
            if (objectsToActivate != null)
            {
                foreach (GameObject obj in objectsToActivate)
                {
                    if (obj != null) obj.SetActive(true);
                }
            }

            // 3. Turn off the dialogue box
            gameObject.SetActive(false);
        }
    }
}