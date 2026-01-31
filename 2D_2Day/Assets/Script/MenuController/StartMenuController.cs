using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour 
{
    public void OnstartClick()
    {
        SceneManager.LoadScene("WaterTest");
    }
    public void OnExitClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
