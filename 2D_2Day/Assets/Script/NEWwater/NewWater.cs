﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿using System.Collections;
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
    public BeerGlass targetGlass;
    public bool autoDetectGlass = true;
    public float glassDetectRadius = 2f;
    public bool startOn = false;

    public bool requireFirstPress = true;
    private float lastLength;
    private bool isEmitting;
    private bool desiredEmitting;

    private bool hasPressedFirst;
    void Start()
    {
        desiredEmitting = startOn;
        desiredEmitting = false;
        hasPressedFirst = !requireFirstPress && startOn;
        lineRenderer.positionCount = 2;
        for (int i = 0; i < 2; i++)
        {
            lineRenderer.SetPosition(i, transform.position);
        }
        lineRenderer.widthMultiplier = 0f;
        isEmitting = false;
        boxCollider2D.enabled = false;
        if (splashEffectObject != null) splashEffectObject.SetActive(false);
        if (autoDetectGlass && targetGlass == null)
        {
            BeerGlass nearest = null;
            float best = float.PositiveInfinity;
            var all = UnityEngine.Object.FindObjectsOfType<BeerGlass>();
            foreach (var g in all)
            {
                if (g == null || g.beerSpriteTransform == null) continue;
                var p = g.beerSpriteTransform.position;
                float d = Mathf.Abs(p.x - transform.position.x) + Mathf.Max(0f, transform.position.y - p.y);
                if (d < best && Vector2.Distance(transform.position, p) <= glassDetectRadius)
                {
                    best = d;
                    nearest = g;
                }
            }
            if (nearest != null) targetGlass = nearest;
        }
    }

    void Update()
    {
        if (requireFirstPress && !hasPressedFirst)
        {
            if (Input.GetKeyDown(KeyCode.Space)) hasPressedFirst = true;
            desiredEmitting = false;
        }
        else
        {
            desiredEmitting = Input.GetKey(KeyCode.Space);
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
        else
        {
            ResizeLine(0f);
            RescaleCollider(0f);
        }
    }

    private void GetCurrentLength(out float currentMaxLength)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, maxLength, obstacleLayerMask);
        if (hit)
        {
            currentMaxLength = Vector3.Distance(transform.position, hit.point);

            if (isEmitting)
            {
                // 1. ลองหาสคริปต์จากตัวที่ถูกชนตรงๆ ก่อน
                BeerGlass glass = hit.collider.GetComponent<BeerGlass>();

                // 2. ถ้าไม่เจอ (เช่น ชนผิวน้ำเบียร์ แต่สคริปต์อยู่ก้นแก้ว) ให้หาจากวัตถุข้างเคียงใน Parent เดียวกัน
                if (glass == null && hit.collider.transform.parent != null)
                {
                    glass = hit.collider.transform.parent.GetComponentInChildren<BeerGlass>();
                }

                if (glass != null)
                {
                    glass.ReceiveWater(Time.deltaTime);
                }
            }
        }
        else
        {
            currentMaxLength = maxLength;
        }

        if (targetGlass != null && targetGlass.beerSpriteTransform != null)
        {
            var sr = targetGlass.beerSpriteTransform.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                var b = sr.bounds;
                float x0 = lineRenderer.GetPosition(0).x;
                if (x0 >= b.min.x && x0 <= b.max.x)
                {
                    float glassTopY = b.max.y;
                    float toTop = Mathf.Max(0f, transform.position.y - glassTopY);
                    currentMaxLength = Mathf.Min(currentMaxLength, toTop);
                }
            }
        }
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
