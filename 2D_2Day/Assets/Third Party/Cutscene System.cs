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

    private List<DialogueLine> currentLines;
    private int currentIndex = 0;

    private void Awake() => Instance = this;

    public void StartCutscene(List<DialogueLine> lines)
    {
        currentLines = lines;
        currentIndex = 0;
        dialoguePanel.SetActive(true);
        DisplayLine();
    }

    public void DisplayLine()
    {
        if (currentIndex < currentLines.Count)
        {
            DialogueLine line = currentLines[currentIndex];

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

    public void OnClickNext() // เรียกใช้เมื่อคลิกหน้าจอ
    {
        if (currentLines[currentIndex].hasChoices) return; // ถ้ามีตัวเลือก ต้องกดเลือกก่อนถึงจะไปต่อได้

        currentIndex++;
        DisplayLine();
    }

    private void ShowChoices(DialogueChoice[] choices)
    {
        // สร้างปุ่มตามจำนวน Choice และกำหนด index ที่จะกระโดดไป
        foreach (var choice in choices)
        {
            Button btn = Instantiate(choiceButtonPrefab, choiceParent.transform);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = choice.choiceText;
            btn.onClick.AddListener(() => {
                currentIndex = choice.nextLineIndex; // กระโดดข้าม Line ตามที่กำหนด [00:08:05]
                ClearChoices();
                DisplayLine();
            });
        }
    }

    private void ClearChoices() { /* Code สำหรับลบปุ่มเก่าออก */ }
    private void EndCutscene() { dialoguePanel.SetActive(false); }
}
