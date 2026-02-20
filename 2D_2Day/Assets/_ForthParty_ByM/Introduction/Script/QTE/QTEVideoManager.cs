using UnityEngine;
using UnityEngine.Video;
using System.Collections.Generic;

public class QTEVideoManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject qteCanvas;

    [Header("QTE Settings")]
    public List<double> qteTriggerTimes;

    private int currentQTEIndex = 0;

    void Update()
    {
        
        if (currentQTEIndex < qteTriggerTimes.Count)
        {
            if (videoPlayer.time >= qteTriggerTimes[currentQTEIndex])
            {
                TriggerQTE();
            }
        }
    }

    void TriggerQTE()
    {
        qteCanvas.SetActive(true);
        currentQTEIndex++;
    }
}