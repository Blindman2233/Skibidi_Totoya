using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Call ClearSaveData on DataPersistenceManager. Use for testing or in Option menu.
/// Assign a Button and it will clear all save slots when clicked.
/// </summary>
public class ClearSaveDataButton : MonoBehaviour
{
    public Button button;

    private void OnEnable()
    {
        if (button == null)
            button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(ClearSaves);
        }
    }

    public void ClearSaves()
    {
        if (DataPersistenceManager.Instance != null)
        {
            DataPersistenceManager.Instance.ClearSaveData();
            Debug.Log("Save data cleared. Use New Game or Load to continue.");
        }
    }
}
