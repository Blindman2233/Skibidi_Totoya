using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewWater : MonoBehaviour
{
    public float maxLength;
    public float fallSpeed;
    public LineRenderer lineRenderer;
    public LayerMask obstacleLayerMask;
    public BoxCollider2D boxCollider2D;
    public GameObject splashEffectObject;
    public float maxWidth = 1f;
    public float widthChangeSpeed = 6f;
    public float enableThreshold = 0.9f;
    public float disableThreshold = 0.05f;

    private float lastLength;
    private bool isEmitting;
    private bool desiredEmitting;


    // Start is called before the first frame update
    void Start()
    {
        lineRenderer.positionCount = 2;
        for (int i = 0; i < 2; i++)
        {
            lineRenderer.SetPosition(i, transform.position);
        }
        lineRenderer.widthMultiplier = desiredEmitting ? maxWidth : 0f;
        isEmitting = desiredEmitting && lineRenderer.widthMultiplier >= enableThreshold * maxWidth;
        boxCollider2D.enabled = isEmitting;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            desiredEmitting = !desiredEmitting;
        }

        float targetWidth = desiredEmitting ? maxWidth : 0f;
        lineRenderer.widthMultiplier = Mathf.MoveTowards(lineRenderer.widthMultiplier, targetWidth, Time.deltaTime * widthChangeSpeed);

        if (!isEmitting && lineRenderer.widthMultiplier >= enableThreshold * maxWidth)
        {
            isEmitting = true;
            boxCollider2D.enabled = true;
        }
        else if (isEmitting && lineRenderer.widthMultiplier <= disableThreshold * maxWidth)
        {
            isEmitting = false;
            boxCollider2D.enabled = false;
            splashEffectObject.SetActive(false);
            lastLength = 0f;
            ResizeLine(0f);
            RescaleCollider(0f);
        }

        if (isEmitting)
        {
            GetCurrentLength(out float currentMaxLength);
            CalculateActualLength(currentMaxLength, out float currentLength);
            ResizeLine(currentLength);
            RescaleCollider(currentLength);
            CheckForSplashEffect(currentLength, currentMaxLength);
        }
    }

    private void GetCurrentLength(out float currentMaxLength)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, maxLength, obstacleLayerMask);
        if (hit)
            currentMaxLength = Vector3.Distance(transform.position, hit.point);
        else
            currentMaxLength = maxLength;
    }

    private void CalculateActualLength(float currentMaxLength, out float currentLength)
    {
        currentLength = lastLength + Time.deltaTime * fallSpeed;
        currentLength = Mathf.Clamp(currentLength, 0, currentMaxLength);
        lastLength = currentLength;
    }

    private void ResizeLine(float currentLength)
    {
        lineRenderer.SetPosition(1, transform.position - Vector3.up * currentLength);
        lineRenderer.material.SetFloat("_Length", currentLength);
    }

    private void RescaleCollider(float currentLength)
    {
        boxCollider2D.size = new Vector2(boxCollider2D.size.x, currentLength);
        boxCollider2D.offset = new Vector2(0, -currentLength / 2);
    }

    private void CheckForSplashEffect(float currentLength, float currentMaxLength)
    {
        bool showSplash = currentLength >= currentMaxLength && lineRenderer.widthMultiplier >= enableThreshold * maxWidth;
        splashEffectObject.SetActive(showSplash);
        splashEffectObject.transform.position = transform.position - Vector3.up * currentLength;
    }
}
