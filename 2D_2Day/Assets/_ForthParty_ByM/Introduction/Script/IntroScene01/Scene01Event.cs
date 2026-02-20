using System.Collections;
using UnityEngine;

public class Scene01Event : MonoBehaviour
{
    [SerializeField] GameObject fadeScreenIn;

    
    void Start()
    {
        StartCoroutine(EventStarter());
    }

    
    void Update()
    {
        
    }

    IEnumerator EventStarter()
    {
        yield return new WaitForSeconds(1.5f);
        fadeScreenIn.SetActive(false);
    }
}
