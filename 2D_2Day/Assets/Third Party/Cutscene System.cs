using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CutsceneSystem : MonoBehaviour
{
    public static CutsceneSystem Instance; // Singleton

    [Header("UI References")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Image portraitImage;
    public GameObject dialoguePanel;
    public GameObject choiceParent; // Container สำหรับปุ่มตัวเลือก
    public Button choiceButtonPrefab;

    private List<DialogueLineData> currentLines;
    private int currentIndex = 0;

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

            // อัปเดตข้อมูลบน UI [00:01:31]
            nameText.text = line.characterName;
            dialogueText.text = line.text;
            portraitImage.sprite = line.characterPortrait;

            // ตรวจสอบเรื่องทางเลือก (Dialogue Trees) [00:07:46]
            if (line.hasChoices)
            {
                ShowChoices(line.choices);
            }
        }
        else
        {
            EndCutscene();
        }
    }

    public void OnClickNext()
    {
        if (currentLines == null || currentIndex >= currentLines.Count) return;
        if (currentLines[currentIndex].hasChoices) return;

        currentIndex++;
        DisplayLineData();
    }

    private void ShowChoices(DialogueChoice[] choices)
    {
        ClearChoices(); // ล้างของเก่าก่อนสร้างใหม่
        choiceParent.SetActive(true); // เปิด Panel ตัวเลือก

        foreach (var choice in choices)
        {
            DialogueChoice tempChoice = choice; // สร้างตัวแปรชั่วคราวมารับค่า
            Button btn = Instantiate(choiceButtonPrefab, choiceParent.transform);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = tempChoice.choiceText;

            btn.onClick.AddListener(() => {
                currentIndex = tempChoice.nextLineIndex;
                choiceParent.SetActive(false); // ปิด Panel เมื่อเลือกแล้ว
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
    private void EndCutscene() { dialoguePanel.SetActive(false) ; }
}
