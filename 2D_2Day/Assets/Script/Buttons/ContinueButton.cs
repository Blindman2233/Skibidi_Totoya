using UnityEngine;

public class ContinueButton : MonoBehaviour
{
    [SerializeField] private GameObject[] uisToClose;

    public void CloseUI()
    {
        if (uisToClose != null)
        {
            foreach (GameObject ui in uisToClose)
            {
                if (ui != null)
                {
                    ui.SetActive(false);
                }
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseUI();
        }
    }
}
