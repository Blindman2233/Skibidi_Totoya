using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI textCom;
    public string[] dialogueLines;
    public float textSpeed = 0.05f;

    [Header("Portrait Settings")]
    public Image portraitSlot;
    public Image rightPortraitSlot;
    public Sprite defaultPortrait;
    public Sprite correctPortrait;
    public Sprite wrongPortrait;
    [InspectorName("Left")]
    public Sprite[] linePortraitsLeft;
    public Sprite[] linePortraitsRight;
    public Sprite[] correctLinePortraitsLeft;
    public Sprite[] correctLinePortraitsRight;
    public Sprite[] wrongLinePortraitsLeft;
    public Sprite[] wrongLinePortraitsRight;

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

    // --- NEW DEACTIVATION SETTINGS ---
    [Header("Deactivation Settings")]
    [Tooltip("Objects to turn OFF when dialogue starts")]
    public GameObject[] deactivateBefore;
    [Tooltip("Objects to turn OFF when dialogue finishes")]
    public GameObject[] deactivateAfter;
    // ---------------------------------

    private int index;
    private bool isChoosing = false;
    private bool isShowingResult = false;
    private WaitForSeconds delay;
    private Sprite[] _currentPortraits;
    private Sprite _fallbackPortrait;
    private Sprite[] _currentRightPortraits;
    private Sprite _fallbackRightPortrait;

    void Start()
    {
        delay = new WaitForSeconds(textSpeed);
        textCom.text = string.Empty;

        if (portraitSlot != null && defaultPortrait != null)
        {
            portraitSlot.sprite = defaultPortrait;
        }

        if (choicePanel != null) choicePanel.SetActive(false);

        // Handle initial activations/deactivations
        ToggleGroup(activateBefore, true);
        ToggleGroup(deactivateBefore, false); // NEW: Turns off objects at start
        ToggleGroup(activateAfter, false);

        _currentPortraits = linePortraitsLeft;
        _fallbackPortrait = defaultPortrait;
        _currentRightPortraits = linePortraitsRight;
        _fallbackRightPortrait = defaultPortrait;
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

        if (portraitSlot != null)
        {
            Sprite s = null;
            if (_currentPortraits != null && index >= 0 && index < _currentPortraits.Length)
            {
                s = _currentPortraits[index];
            }
            portraitSlot.sprite = s != null ? s : _fallbackPortrait;
        }
        if (rightPortraitSlot != null)
        {
            Sprite sr = null;
            if (_currentRightPortraits != null && index >= 0 && index < _currentRightPortraits.Length)
            {
                sr = _currentRightPortraits[index];
            }
            rightPortraitSlot.sprite = sr != null ? sr : _fallbackRightPortrait;
        }

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

        _currentPortraits = correctLinePortraitsLeft;
        _fallbackPortrait = correctPortrait;
        _currentRightPortraits = correctLinePortraitsRight;
        _fallbackRightPortrait = correctPortrait;
        StartDialogue(correctResponse);
    }

    public void SelectWrong()
    {
        isShowingResult = true;
        choicePanel.SetActive(false);

        _currentPortraits = wrongLinePortraitsLeft;
        _fallbackPortrait = wrongPortrait;
        _currentRightPortraits = wrongLinePortraitsRight;
        _fallbackRightPortrait = wrongPortrait;
        StartDialogue(wrongResponse);
    }

    void FinishDialogue()
    {
        ToggleGroup(activateAfter, true);
        ToggleGroup(deactivateAfter, false);
        gameObject.SetActive(false);
    }

    // This helper function handles the arrays to prevent errors
    void ToggleGroup(GameObject[] group, bool state)
    {
        if (group == null) return;
        foreach (GameObject obj in group)
        {
            if (obj != null) obj.SetActive(state);
        }
    }
}
