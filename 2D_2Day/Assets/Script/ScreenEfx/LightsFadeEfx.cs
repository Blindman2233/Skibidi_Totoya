using UnityEngine;
using UnityEngine.UI;

public class LightsFadeEfx : MonoBehaviour
{
    [Header("UI Target")]
    public CanvasGroup canvasGroup;
    public Image blackImage;

    [Header("Fade Settings")]
    [Range(0f, 1f)] public float lightAlpha = 0f;
    [Range(0f, 1f)] public float darkAlpha = 1f;
    public float fadeInDuration = 0.35f;
    public float darkHoldDuration = 0.6f;
    public float fadeOutDuration = 0.35f;
    public float lightHoldDuration = 1.2f;

    [Header("Behavior")]
    public bool autoStart = true;
    public bool loop = true;
    public bool ignoreTimeScale = false;
    public bool randomBursts = false;
    public Vector2 initialDelayRange = new Vector2(0.5f, 2f);
    public Vector2 betweenBurstsRange = new Vector2(1f, 3f);

    bool _running;
    Coroutine _routine;

    void Awake()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        if (blackImage == null) blackImage = GetComponent<Image>();
        SetAlpha(lightAlpha);
        DisableRaycast();
    }

    void OnEnable()
    {
        if (autoStart && _routine == null)
        {
            if (randomBursts)
            {
                _routine = StartCoroutine(RandomLoop());
            }
            else
            {
                _routine = StartCoroutine(FadeLoop(loop));
            }
        }
    }

    void OnDisable()
    {
        _running = false;
        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }
        SetAlpha(lightAlpha);
    }

    public void StartLoop()
    {
        loop = true;
        if (_routine == null) _routine = StartCoroutine(FadeLoop(true));
    }

    public void StopLoop()
    {
        loop = false;
        _running = false;
        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }
        SetAlpha(lightAlpha);
    }

    public void TriggerOnce()
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }
        _routine = StartCoroutine(FadeLoop(false));
    }

    System.Collections.IEnumerator FadeLoop(bool repeat)
    {
        _running = true;
        do
        {
            yield return FadeOnce();
        }
        while (repeat && _running);
        _routine = null;
        _running = false;
    }

    System.Collections.IEnumerator RandomLoop()
    {
        _running = true;
        float startDelay = Random.Range(initialDelayRange.x, initialDelayRange.y);
        if (startDelay > 0f) yield return Hold(startDelay);
        while (_running)
        {
            yield return FadeOnce();
            float gap = Random.Range(betweenBurstsRange.x, betweenBurstsRange.y);
            if (gap > 0f) yield return Hold(gap);
        }
        _routine = null;
    }

    System.Collections.IEnumerator FadeOnce()
    {
        yield return LerpAlpha(GetAlpha(), darkAlpha, fadeInDuration);
        yield return Hold(darkHoldDuration);
        yield return LerpAlpha(GetAlpha(), lightAlpha, fadeOutDuration);
        yield return Hold(lightHoldDuration);
    }

    System.Collections.IEnumerator LerpAlpha(float from, float to, float duration)
    {
        if (duration <= 0f)
        {
            SetAlpha(to);
            yield break;
        }
        float t = 0f;
        while (t < 1f)
        {
            t += Step(duration);
            SetAlpha(Mathf.Lerp(from, to, Mathf.Clamp01(t)));
            yield return null;
        }
        SetAlpha(to);
    }

    System.Collections.IEnumerator Hold(float seconds)
    {
        if (seconds <= 0f) yield break;
        float elapsed = 0f;
        while (elapsed < seconds)
        {
            elapsed += Delta();
            yield return null;
        }
    }

    void SetAlpha(float a)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = a;
        }
        else if (blackImage != null)
        {
            var c = blackImage.color;
            c.a = a;
            blackImage.color = c;
        }
    }

    float GetAlpha()
    {
        if (canvasGroup != null) return canvasGroup.alpha;
        if (blackImage != null) return blackImage.color.a;
        return 0f;
    }

    void DisableRaycast()
    {
        if (blackImage != null) blackImage.raycastTarget = false;
    }

    float Delta() => ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
    float Step(float duration) => (ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime) / Mathf.Max(duration, 0.0001f);
}
