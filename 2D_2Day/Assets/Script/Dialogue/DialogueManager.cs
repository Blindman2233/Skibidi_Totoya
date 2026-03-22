using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public Image characterIconLeft;
    public Image characterIconRight;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI dialogueArea;

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
    private bool isWaitingForMiniGame = false;

    // เพิ่มตัวแปรสำหรับจัดการ Next Event
    private EventData currentEventData;
    public static event Action<EventData> OnDialogueEnded;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        lines = new Queue<DialogueLine>();
    }

    public void StartDialogue(DialoguesObject dialogueObject)
    {
        StartDialogue(dialogueObject, null);
    }

    public void StartDialogue(DialoguesObject dialogueObject, EventData eventData)
    {
        print("OnStartDialogue");
        isDialogueActive = true;
        isWaitingForChoice = false;
        isWaitingForMiniGame = false;

        // เก็บ EventData ปัจจุบันไว้สำหรับ Next Event
        currentEventData = eventData;

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

        foreach (char letter in dialogueLine.line.ToCharArray())
        {
            dialogueArea.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;

        if (dialogueLine.hasChoices)
        {
            ShowChoices();
        }
        else
        {
            TryStartMiniGame(dialogueLine);
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

    private void TryStartMiniGame(DialogueLine dialogueLine)
    {
        if (dialogueLine == null || dialogueLine.miniGameSettings == null || !dialogueLine.miniGameSettings.triggerPourLiquorMiniGame)
            return;

        if (PourLiquorManager.Instance == null)
        {
            Debug.LogError("DialogueManager: ต้องมี PourLiquorManager ในฉากเพื่อใช้มินิเกมรินเหล้า");
            return;
        }

        StartCoroutine(WaitAndStartMiniGame(dialogueLine));
    }

    private IEnumerator WaitAndStartMiniGame(DialogueLine dialogueLine)
    {
        isWaitingForMiniGame = true;

        float delay = 0.5f;
        if (dialogueLine.miniGameSettings != null)
        {
            delay = Mathf.Max(0f, dialogueLine.miniGameSettings.delayBeforeStart);
        }

        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        // ถ้าระหว่างรอมี Dialogue ใหม่เริ่มหรือสถานะเปลี่ยน ไม่ต้องเริ่มมินิเกม
        if (!isDialogueActive || currentLine != dialogueLine)
        {
            isWaitingForMiniGame = false;
            yield break;
        }

        PourLiquorManager.Instance.StartPourLiquorFromDialogue(
            dialogueLine.miniGameSettings.pourLiquorSettings,
            result =>
            {
                isWaitingForMiniGame = false;
                HandleMiniGameResult(dialogueLine, result);
            });
    }

    private void HandleMiniGameResult(DialogueLine dialogueLine, PourLiquorResult result)
    {
        if (dialogueLine == null)
            return;

        var flags = EventManager.Instance != null ? EventManager.Instance.storyFlags : null;

        if (flags != null)
        {
            if (result == PourLiquorResult.Success)
            {
                foreach (var action in dialogueLine.miniGameSettings.onMiniGameSuccessActions)
                {
                    action.ExecuteAction(flags);
                }
            }
            else if (result == PourLiquorResult.Fail)
            {
                foreach (var action in dialogueLine.miniGameSettings.onMiniGameFailActions)
                {
                    action.ExecuteAction(flags);
                }
            }
        }

        if (result == PourLiquorResult.Success && dialogueLine.miniGameSettings.onMiniGameSuccessDialogue != null)
        {
            StartDialogue(dialogueLine.miniGameSettings.onMiniGameSuccessDialogue);
        }
        else if (result == PourLiquorResult.Fail && dialogueLine.miniGameSettings.onMiniGameFailDialogue != null)
        {
            StartDialogue(dialogueLine.miniGameSettings.onMiniGameFailDialogue);
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

        // สำคัญ! ส่ง Event ให้ระบบอื่นรู้ว่า Dialogue จบแล้ว
        OnDialogueEnded?.Invoke(currentEventData);
        currentEventData = null;
    }

    private void Update()
    {
        // Only allow space to advance if not waiting for choice and not typing
        if (Input.GetKeyDown(KeyCode.Space) && !isWaitingForChoice && !isWaitingForMiniGame && !isTyping)
        {
            DisplayNextDialogueLine();
        }
    }
}
