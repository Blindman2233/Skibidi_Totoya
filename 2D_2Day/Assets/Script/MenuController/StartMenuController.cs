using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuController : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Type the exact name of the scene you want to load")]
    public string sceneToLoad;
    
    [Header("Options Panel")]
    [Tooltip("Assign your Options Panel GameObject here")]
    public GameObject optionsPanel;

    [Header("Windows Mode")]
    public Toggle windowsModeToggle;

    void Start()
    {
        int saved = PlayerPrefs.GetInt("windowedMode", 1);
        bool isWindowed = saved == 1;
        ApplyWindowsMode(isWindowed);
        if (windowsModeToggle != null)
        {
            windowsModeToggle.isOn = isWindowed;
            windowsModeToggle.onValueChanged.AddListener(OnWindowsModeChanged);
        }
    }

    void OnDestroy()
    {
        if (windowsModeToggle != null)
        {
            windowsModeToggle.onValueChanged.RemoveListener(OnWindowsModeChanged);
        }
    }

    public void OnStartClick()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogError("No scene name provided in the Inspector!");
        }
    }

    public void OnExitClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
    
    public void OnOptionsClick()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("Options Panel reference is missing on StartMenuController");
        }
    }
    
    public void OnCloseOptionsClick()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }
    }

    public void OnWindowsModeChanged(bool isWindowed)
    {
        ApplyWindowsMode(isWindowed);
        PlayerPrefs.SetInt("windowedMode", isWindowed ? 1 : 0);
    }

    void ApplyWindowsMode(bool isWindowed)
    {
        Screen.fullScreenMode = isWindowed ? FullScreenMode.Windowed : FullScreenMode.FullScreenWindow;
    }
}
