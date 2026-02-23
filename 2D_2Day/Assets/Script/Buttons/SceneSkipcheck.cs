using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class SceneSkipcheck : MonoBehaviour
{
    [Header("References")]
    public VideoPlayer videoPlayer;
    
    [Header("Settings")]
    [Tooltip("Name of the scene to load. Leave empty to load the next scene in Build Settings.")]
    public string sceneToLoad;

    void Start()
    {
        if (videoPlayer != null)
        {
            // Subscribe to the event that fires when the video finishes
            videoPlayer.loopPointReached += OnVideoFinished;
        }
        else
        {
            Debug.LogWarning("VideoPlayer not assigned in SceneSkipcheck!");
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        LoadNextScene();
    }

    void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            // Load the next scene in the build index
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            
            // Check if the next index is valid
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                Debug.LogWarning("No next scene found in Build Settings!");
            }
        }
    }

    void OnDestroy()
    {
        // Always unsubscribe from events to prevent memory leaks
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }
}
