using UnityEngine;
using UnityEngine.UI;

public class LightsFadeEfx : MonoBehaviour
{
    public enum FadeType
    {
        PingPong,       // Smooth loop: Min -> Max -> Min
        RandomNoise,    // Random flickering (Perlin Noise)
        FadeIn,         // Min -> Max (Once)
        FadeOut,        // Max -> Min (Once)
        RandomInterval  // Randomly switches between Min and Max with random delays
    }

    [Header("Target Object")]
    [Tooltip("ลาก Object ที่ต้องการให้ Fade มาใส่ (ถ้าไม่ใส่จะใช้ Object นี้)")]
    public GameObject targetObject;

    [Header("Fade Settings")]
    [Tooltip("รูปแบบการ Fade")]
    public FadeType fadeType = FadeType.PingPong;

    [Tooltip("ค่าความจาง/เข้ม ต่ำสุด (Min Opacity)")]
    public float minVal = 0.0f;
    [Tooltip("ค่าความจาง/เข้ม สูงสุด (Max Opacity)")]
    public float maxVal = 1.0f;
    
    [Tooltip("ความเร็วในการเปลี่ยนค่า (Speed)")]
    public float fadeSpeed = 1.0f;

    [Header("Random Interval Settings (เฉพาะโหมด RandomInterval)")]
    [Tooltip("เวลาที่แสดงผล (Visible Duration Min)")]
    public float minVisibleTime = 0.5f;
    [Tooltip("เวลาที่แสดงผล (Visible Duration Max)")]
    public float maxVisibleTime = 2.0f;
    [Tooltip("เวลาที่จางหาย (Invisible Duration Min)")]
    public float minInvisibleTime = 0.5f;
    [Tooltip("เวลาที่จางหาย (Invisible Duration Max)")]
    public float maxInvisibleTime = 2.0f;

    private SpriteRenderer spriteRenderer;
    private Light lightSource;
    private Image uiImage;
    private CanvasGroup canvasGroup;
    
    private float timeOffset;
    private float timer;
    
    // Variables for RandomInterval
    private float targetAlpha;
    private float currentAlpha;
    private float nextStateChangeTime;
    private bool isTargetMax; // Are we currently aiming for MaxVal?

    void Start()
    {
        // Determine which object to use
        GameObject objToUse = targetObject != null ? targetObject : gameObject;

        // Auto-detect components to fade on the target object
        spriteRenderer = objToUse.GetComponent<SpriteRenderer>();
        lightSource = objToUse.GetComponent<Light>();
        uiImage = objToUse.GetComponent<Image>();
        canvasGroup = objToUse.GetComponent<CanvasGroup>();

        // Random offset prevents multiple objects from fading in perfect sync
        timeOffset = Random.Range(0f, 100f);
        
        // Initialize value based on mode
        if (fadeType == FadeType.FadeIn) 
        {
            currentAlpha = minVal;
            SetAlpha(minVal);
        }
        else if (fadeType == FadeType.FadeOut) 
        {
            currentAlpha = maxVal;
            SetAlpha(maxVal);
        }
        else if (fadeType == FadeType.RandomInterval)
        {
            // Start visible
            currentAlpha = maxVal;
            targetAlpha = maxVal;
            isTargetMax = true;
            ScheduleNextStateChange(true);
            SetAlpha(maxVal);
        }
        else
        {
            currentAlpha = maxVal;
        }
    }

    void Update()
    {
        float finalValue = currentAlpha;

        switch (fadeType)
        {
            case FadeType.PingPong:
                // Smoothly bounces between 0 and 1
                float t = Mathf.PingPong(Time.time * fadeSpeed + timeOffset, 1f);
                finalValue = Mathf.Lerp(minVal, maxVal, t);
                break;

            case FadeType.RandomNoise:
                // Smooth random noise
                float noise = Mathf.PerlinNoise(Time.time * fadeSpeed + timeOffset, 0f);
                finalValue = Mathf.Lerp(minVal, maxVal, noise);
                break;

            case FadeType.FadeIn:
                timer += Time.deltaTime * fadeSpeed;
                float progressIn = Mathf.Clamp01(timer);
                finalValue = Mathf.Lerp(minVal, maxVal, progressIn);
                break;

            case FadeType.FadeOut:
                timer += Time.deltaTime * fadeSpeed;
                float progressOut = Mathf.Clamp01(timer);
                finalValue = Mathf.Lerp(maxVal, minVal, progressOut);
                break;

            case FadeType.RandomInterval:
                // Check if it's time to switch state
                if (Time.time >= nextStateChangeTime)
                {
                    // Toggle state
                    isTargetMax = !isTargetMax;
                    targetAlpha = isTargetMax ? maxVal : minVal;
                    ScheduleNextStateChange(isTargetMax);
                }

                // Smoothly move currentAlpha towards targetAlpha
                // We use MoveTowards to ensure constant speed
                finalValue = Mathf.MoveTowards(currentAlpha, targetAlpha, Time.deltaTime * fadeSpeed);
                currentAlpha = finalValue;
                break;
        }

        SetAlpha(finalValue);
    }

    void ScheduleNextStateChange(bool isVisible)
    {
        float waitTime = 0f;
        if (isVisible)
        {
            // How long to stay visible
            waitTime = Random.Range(minVisibleTime, maxVisibleTime);
        }
        else
        {
            // How long to stay invisible
            waitTime = Random.Range(minInvisibleTime, maxInvisibleTime);
        }
        nextStateChangeTime = Time.time + waitTime;
    }

    // Helper function to apply alpha/intensity to any supported component
    void SetAlpha(float value)
    {
        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = value;
            spriteRenderer.color = c;
        }
        else if (lightSource != null)
        {
            lightSource.intensity = value;
        }
        else if (uiImage != null)
        {
            Color c = uiImage.color;
            c.a = value;
            uiImage.color = c;
        }
        else if (canvasGroup != null)
        {
            canvasGroup.alpha = value;
        }
    }
    
    // Public method to reset/restart fade
    public void RestartFade()
    {
        timer = 0f;
        if (fadeType == FadeType.FadeIn) SetAlpha(minVal);
        else if (fadeType == FadeType.FadeOut) SetAlpha(maxVal);
    }
}
