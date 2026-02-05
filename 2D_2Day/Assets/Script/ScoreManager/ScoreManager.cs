using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance; // Easy access for other scripts
    public TextMeshProUGUI scoreText;
    public int score = 0;

    void Awake() { instance = this; }

    void Start() { UpdateUI(); }

    public void ChangeScore(int amount)
    {
        score += amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText != null) scoreText.text = "Score: " + score;
    }
}