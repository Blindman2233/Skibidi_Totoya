using System.Collections;
using UnityEngine;

public class StartQTE : MonoBehaviour
{
    [SerializeField] GameObject qteUI;

    void Start()
    {
        StartCoroutine(EventQTE());
    }


    void Update()
    {

    }

    IEnumerator EventQTE()
    {
        yield return new WaitForSeconds(12f);
        qteUI.SetActive(true);
    }
}
