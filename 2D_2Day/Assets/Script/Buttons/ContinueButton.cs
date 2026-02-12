using UnityEngine;

public class ContinueButton : MonoBehaviour
{
    public GameObject winUI;

    void Awake()
    {
        if (winUI == null) winUI = gameObject;
    }

    public void OnClickContinue()
    {
        Time.timeScale = 1f;
        if (winUI != null) winUI.SetActive(false);
    }
}
