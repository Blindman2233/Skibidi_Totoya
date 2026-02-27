using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public Image characterIconLeft;
    public Image characterIconRight;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI dialogueArea;

    [Header("Audio")]
    public AudioSource audioSource;
    private DialoguesObject currentDialogue;

    [Header("Choice UI")]
    public GameObject choicePanel;
    public GameObject choiceButtonPrefab;
    public Transform choiceButtonContainer;

    private Queue<DialogueLine> lines;
    private DialogueLine currentLine;

    public bool isDialogueActive = false;

    public float typingSpeed = 0.05f;
    public GameObject dialogueUI;

    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private bool isWaitingForChoice = false;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        lines = new Queue<DialogueLine>();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                // Try to add one if missing
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }

    public void StartDialogue(DialoguesObject dialogueObject)
    {
        currentDialogue = dialogueObject;
        isDialogueActive = true;
        isWaitingForChoice = false;

        // --- Handle Initial Activations/Deactivations ---
        ToggleObjects(currentDialogue.activateBefore, true);
        ToggleObjects(currentDialogue.deactivateBefore, false);
        // ------------------------------------------------

        // Check if we have a valid sound, either in DialogueData or already on the AudioSource
        bool hasSound = (currentDialogue.dialogueSound != null) || (audioSource != null && audioSource.clip != null);
        
        if (!hasSound)
        {
            Debug.LogWarning($"[DialogueManager] No audio clip assigned in Dialogue Data: {dialogueObject.name} AND no default clip on AudioSource. Please assign a sound.");
        }

        lines.Clear();

        foreach (DialogueLine line in dialogueObject.lines)
        {
            lines.Enqueue(line);
        }

        dialogueUI.SetActive(true);
        choicePanel.SetActive(false);

        DisplayNextDialogueLine();
    }

    public void DisplayNextDialogueLine()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            dialogueArea.text = currentLine.line;
            isTyping = false;
            
            if (audioSource != null) audioSource.Stop();
            
            if (currentLine.hasChoices)
            {
                ShowChoices();
            }
            return;
        }

        // Don't advance if waiting for choice
        if (isWaitingForChoice)
            return;

        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        currentLine = lines.Dequeue();

        characterName.text = currentLine.character.name;
        characterIconRight.gameObject.SetActive(false);
        characterIconLeft.gameObject.SetActive(false);
        
        choicePanel.SetActive(false);

        if (currentLine.side == DialogueSide.Solo)
        {
            if (currentLine.character.icon2 == null)
            {
                characterIconRight.sprite = currentLine.character.icon;
                characterIconRight.gameObject.SetActive(true);
            }
            else
            {
                characterIconLeft.sprite = currentLine.character.icon2;
                characterIconLeft.gameObject.SetActive(true);
            }
        }

        else if (currentLine.side == DialogueSide.Both)
        {
            characterIconRight.sprite = currentLine.character.icon;
            characterIconLeft.sprite = currentLine.character.icon2;
            
            characterIconRight.gameObject.SetActive(true);
            characterIconLeft.gameObject.SetActive(true);
        }

        StopAllCoroutines();

        typingCoroutine = StartCoroutine(TypeSentence(currentLine));
    }

    IEnumerator TypeSentence(DialogueLine dialogueLine)
    {
        isTyping = true;

        dialogueArea.text = "";

        int charCount = 0;
        foreach (char letter in dialogueLine.line.ToCharArray())
        {
            dialogueArea.text += letter;
            charCount++;

            if (audioSource != null && currentDialogue != null)
            {
                // Determine which clip to play: DialogueData specific sound OR fallback to AudioSource's default clip
                AudioClip clipToPlay = currentDialogue.dialogueSound != null ? currentDialogue.dialogueSound : audioSource.clip;
                
                if (clipToPlay != null)
                {
                    int frequency = Mathf.Max(1, currentDialogue.soundFrequency);
                    if (letter != ' ' && charCount % frequency == 0)
                    {
                        audioSource.pitch = Random.Range(currentDialogue.minPitch, currentDialogue.maxPitch);
                        audioSource.clip = clipToPlay; // Ensure the correct clip is assigned
                        audioSource.Play();
                    }
                }
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        
        if (dialogueLine.hasChoices)
        {
            ShowChoices();
        }
    }
    
    private void ShowChoices()
    {
        if (choiceButtonContainer == null || choicePanel == null)
        {
            Debug.LogError("กรุณา Setup Choice Panel/Container ใน Inspector!");
            return;
        }
        
        isWaitingForChoice = true;
        choicePanel.SetActive(true);

        // Clear all existing choice buttons
        foreach (Transform child in choiceButtonContainer)
        {
            Destroy(child.gameObject);
        }

        // Create new choice buttons with proper positioning
        for (int i = 0; i < currentLine.choices.Count; i++)
        {
            GameObject choiceButton = Instantiate(choiceButtonPrefab, choiceButtonContainer);
            
            // Set proper position for each button
            RectTransform buttonRect = choiceButton.GetComponent<RectTransform>();
            if (buttonRect != null)
            {
                buttonRect.anchoredPosition = new Vector2(0, -i * 60); // Space buttons 60 units apart vertically
                buttonRect.anchorMin = new Vector2(0.5f, 1); // Center horizontally, anchor to top
                buttonRect.anchorMax = new Vector2(0.5f, 1);
                buttonRect.pivot = new Vector2(0.5f, 1);
            }
            
            Button button = choiceButton.GetComponent<Button>();
            TextMeshProUGUI buttonText = choiceButton.GetComponentInChildren<TextMeshProUGUI>();
            
            if (buttonText != null)
                buttonText.text = currentLine.choices[i].choiceText;
            
            int choiceIndex = i;
            button.onClick.AddListener(() => OnChoiceSelected(choiceIndex));
        }
        
        // Adjust container size to fit all buttons
        RectTransform containerRect = choiceButtonContainer.GetComponent<RectTransform>();
        if (containerRect != null)
        {
            float totalHeight = currentLine.choices.Count * 60;
            containerRect.sizeDelta = new Vector2(containerRect.sizeDelta.x, totalHeight);
        }
    }
    
    private void OnChoiceSelected(int choiceIndex)
    {
        isWaitingForChoice = false;
        
        DialogueChoice selectedChoice = currentLine.choices[choiceIndex];
        
        // Execute consequences
        foreach (var action in selectedChoice.consequences)
        {
            if (EventManager.Instance != null)
                action.ExecuteAction(EventManager.Instance.storyFlags);
        }
        
        // Clear choice panel
        choicePanel.SetActive(false);
        
        // Clear choice buttons
        foreach (Transform child in choiceButtonContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Handle next action
        if (selectedChoice.triggerEvent != null && EventManager.Instance != null)
        {
            EventManager.Instance.TriggerEvent(selectedChoice.triggerEvent);
        }
        else if (selectedChoice.nextDialogue != null)
        {
            StartDialogue(selectedChoice.nextDialogue);
        }
        else
        {
            DisplayNextDialogueLine();
        }
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        isWaitingForChoice = false;
        dialogueUI.SetActive(false);
        choicePanel.SetActive(false);
        
        // Clear choice buttons
        if (choiceButtonContainer != null)
        {
            foreach (Transform child in choiceButtonContainer)
            {
                Destroy(child.gameObject);
            }
        }

        // --- Handle Final Activations/Deactivations ---
        if (currentDialogue != null)
        {
            ToggleObjects(currentDialogue.activateAfter, true);
            ToggleObjects(currentDialogue.deactivateAfter, false);
        }
        // ---------------------------------------------
    }

    private void ToggleObjects(string[] objectNames, bool state)
    {
        if (objectNames == null) return;
        
        foreach (string name in objectNames)
        {
            if (string.IsNullOrEmpty(name)) continue;

            GameObject obj = FindObjectByName(name);
            if (obj != null)
            {
                obj.SetActive(state);
            }
            else
            {
                Debug.LogWarning($"[DialogueManager] Could not find object named '{name}' to set active: {state}");
            }
        }
    }

    // Helper to find objects even if they are inactive
    private GameObject FindObjectByName(string name)
    {
        // 1. Try standard Find first (fastest, but only works for active objects)
        GameObject obj = GameObject.Find(name);
        if (obj != null) return obj;

        // 2. If not found, we need to search through all objects including inactive ones
        // Note: This can be slow, so use sparingly
        foreach (GameObject root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            obj = FindInHierarchy(root.transform, name);
            if (obj != null) return obj;
        }
        
        return null;
    }

    private GameObject FindInHierarchy(Transform parent, string name)
    {
        if (parent.name == name) return parent.gameObject;
        
        foreach (Transform child in parent)
        {
            GameObject result = FindInHierarchy(child, name);
            if (result != null) return result;
        }
        return null;
    }

    private void Update()
    {
        // Allow space to advance or skip typing if not waiting for choice
        if (Input.GetKeyDown(KeyCode.Space) && !isWaitingForChoice)
        {
            DisplayNextDialogueLine();
        }
    }
}
