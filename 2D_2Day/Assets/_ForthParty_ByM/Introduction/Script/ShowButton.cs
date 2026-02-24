using System.Collections;
using UnityEngine;

public class ShowButton : MonoBehaviour
{
    [SerializeField] GameObject buttonUI;
    void Start()
    {
        StartCoroutine(SButton());
    }


    void Update()
    {

    }

    IEnumerator SButton()
    {
        yield return new WaitForSeconds(42f);
        buttonUI.SetActive(true);
    }
}
