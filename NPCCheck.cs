using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCCheck : MonoBehaviour
{
    [Header("Scene Settings")]
    public GameObject NPCTalk;

    public void Interact()
    {
        NPCTalk.SetActive(true);
    }

    // --- SHOW / HIDE PROMPT AUTOMATICALLY ---

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }
}