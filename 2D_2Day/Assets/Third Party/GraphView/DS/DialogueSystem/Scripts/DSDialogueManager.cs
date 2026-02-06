using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DS.ScriptableObjects;
using DS.Data;

namespace DS
{
    public class DSDialogueManager : MonoBehaviour
    {
        public static DSDialogueManager Instance { get; private set; }

        [Header("UI References")]
        public GameObject dialoguePanel;
        public TextMeshProUGUI characterNameText;
        public TextMeshProUGUI dialogueText;
        public Image characterPortrait;
        public GameObject choicesPanel;
        public GameObject choiceButtonPrefab;

        [Header("Audio")]
        public AudioSource audioSource;
        public float textSpeed = 0.05f;

        private DSDialogueSO currentDialogue;
        private bool isTyping = false;
        private Coroutine typingCoroutine;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            HideDialogue();
        }

        public void StartDialogue(DSDialogueSO dialogue)
        {
            if (dialogue == null)
            {
                Debug.LogError("Dialogue is null!");
                return;
            }

            currentDialogue = dialogue;
            ShowDialogue();
            DisplayDialogue();
        }

        public void StartDialogueFromContainer(DSDialogueContainerSO container, string dialogueName)
        {
            if (container == null)
            {
                Debug.LogError("Dialogue container is null!");
                return;
            }

            // ถ้าไม่ระบุ dialogueName ให้เริ่มจาก starting dialogue แรก
            if (string.IsNullOrEmpty(dialogueName))
            {
                DSDialogueSO startingDialogue = FindStartingDialogue(container);
                if (startingDialogue != null)
                {
                    StartDialogue(startingDialogue);
                }
                else
                {
                    Debug.LogError("No starting dialogue found in container!");
                }
            }
            else
            {
                // ค้นหา dialogue ที่ระบุชื่อ
                DSDialogueSO dialogue = FindDialogueInContainer(container, dialogueName);
                if (dialogue != null)
                {
                    StartDialogue(dialogue);
                }
                else
                {
                    Debug.LogError($"Dialogue '{dialogueName}' not found in container!");
                }
            }
        }

        private DSDialogueSO FindStartingDialogue(DSDialogueContainerSO container)
        {
            // ค้นหา starting dialogue ใน grouped dialogues
            foreach (var group in container.DialogueGroups.Keys)
            {
                var dialogues = container.DialogueGroups[group];
                foreach (var dialogue in dialogues)
                {
                    if (dialogue.IsStartingDialogue)
                    {
                        return dialogue;
                    }
                }
            }

            // ค้นหา starting dialogue ใน ungrouped dialogues
            foreach (var dialogue in container.UngroupedDialogues)
            {
                if (dialogue.IsStartingDialogue)
                {
                    return dialogue;
                }
            }

            return null;
        }

        private DSDialogueSO FindDialogueInContainer(DSDialogueContainerSO container, string dialogueName)
        {
            // ค้นหาใน grouped dialogues
            foreach (var group in container.DialogueGroups.Keys)
            {
                var dialogues = container.DialogueGroups[group];
                foreach (var dialogue in dialogues)
                {
                    if (dialogue.DialogueName == dialogueName)
                    {
                        return dialogue;
                    }
                }
            }

            // ค้นหาใน ungrouped dialogues
            foreach (var dialogue in container.UngroupedDialogues)
            {
                if (dialogue.DialogueName == dialogueName)
                {
                    return dialogue;
                }
            }

            return null;
        }

        private void DisplayDialogue()
        {
            if (currentDialogue == null) return;

            // แสดงชื่อตัวละคร (ถ้ามี)
            if (characterNameText != null)
            {
                characterNameText.text = currentDialogue.DialogueName;
            }

            // แสดงรูปตัวละคร (ถ้ามี)
            if (characterPortrait != null && currentDialogue.CharacterArt != null)
            {
                characterPortrait.sprite = currentDialogue.CharacterArt;
                characterPortrait.gameObject.SetActive(true);
            }
            else if (characterPortrait != null)
            {
                characterPortrait.gameObject.SetActive(false);
            }

            // เล่นเสียง (ถ้ามี)
            if (audioSource != null && currentDialogue.VoiceClip != null)
            {
                audioSource.PlayOneShot(currentDialogue.VoiceClip);
            }

            // แสดงข้อความแบบ typing effect
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            typingCoroutine = StartCoroutine(TypeText(currentDialogue.Text));
        }

        private IEnumerator TypeText(string text)
        {
            isTyping = true;
            dialogueText.text = "";

            foreach (char c in text)
            {
                dialogueText.text += c;
                yield return new WaitForSeconds(textSpeed);
            }

            isTyping = false;

            // แสดงตัวเลือก (ถ้ามี)
            if (currentDialogue.Choices != null && currentDialogue.Choices.Count > 0)
            {
                ShowChoices();
            }
        }

        private void ShowChoices()
        {
            if (choicesPanel == null || choiceButtonPrefab == null) return;

            // ลบ choice buttons เก่า
            foreach (Transform child in choicesPanel.transform)
            {
                Destroy(child.gameObject);
            }

            choicesPanel.SetActive(true);

            foreach (var choice in currentDialogue.Choices)
            {
                GameObject choiceButton = Instantiate(choiceButtonPrefab, choicesPanel.transform);
                TextMeshProUGUI buttonText = choiceButton.GetComponentInChildren<TextMeshProUGUI>();
                
                if (buttonText != null)
                {
                    buttonText.text = choice.Text;
                }

                Button button = choiceButton.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.AddListener(() => OnChoiceSelected(choice));
                }
            }
        }

        private void OnChoiceSelected(DSDialogueChoiceData choice)
        {
            // เปลี่ยนคะแนนความดี (ถ้ามีระบบ)
            // PlayerStats.Instance.ChangeGoodnessPoints(choice.GoodnessPointsDelta);

            // ไปยัง dialogue ถัดไป
            if (choice.NextDialogue != null)
            {
                currentDialogue = choice.NextDialogue;
                HideChoices();
                DisplayDialogue();
            }
            else
            {
                EndDialogue();
            }
        }

        public void OnNextButtonPressed()
        {
            if (isTyping)
            {
                // Skip typing
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                }
                dialogueText.text = currentDialogue.Text;
                isTyping = false;

                // แสดงตัวเลือก (ถ้ามี)
                if (currentDialogue.Choices != null && currentDialogue.Choices.Count > 0)
                {
                    ShowChoices();
                }
            }
            else if (currentDialogue.Choices == null || currentDialogue.Choices.Count == 0)
            {
                // ไม่มีตัวเลือก = จบ dialogue
                EndDialogue();
            }
        }

        public void EndDialogue()
        {
            HideDialogue();
            currentDialogue = null;
        }

        private void ShowDialogue()
        {
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(true);
            }
        }

        private void HideDialogue()
        {
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }
            HideChoices();
        }

        private void HideChoices()
        {
            if (choicesPanel != null)
            {
                choicesPanel.SetActive(false);
            }
        }

        private void Update()
        {
            // กด Space/Enter เพื่อข้างหน้า
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                if (dialoguePanel.activeSelf)
                {
                    OnNextButtonPressed();
                }
            }

            // กด Escape เพื่อปิด dialogue
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (dialoguePanel.activeSelf)
                {
                    EndDialogue();
                }
            }
        }
    }
}
