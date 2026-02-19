using UnityEngine;

public class ContinueButton : MonoBehaviour
{
    public GameObject winUI;
    public string targetUIName = "PourBeerUI";

    void Awake()
    {
        if (winUI == null)
        {
            var found = GameObject.Find(targetUIName);
            winUI = found != null ? found : gameObject;
        }
    }

    public void OnClickContinue()
    {
        Time.timeScale = 1f;
        if (winUI == null)
        {
            var found = GameObject.Find(targetUIName);
            winUI = found != null ? found : gameObject;
        }
        if (winUI != null) winUI.SetActive(false);
    }
}
