using UnityEngine;

public class CamShakeEfx : MonoBehaviour
{
    public Transform target;
    public float defaultDuration = 0.4f;
    public float defaultIntensity = 0.25f;
    public float frequency = 25f;
    public bool ignoreTimeScale = false;
    public bool autoStart = true;
    public bool randomBursts = true;
    public bool loop = true;
    public Vector2 initialDelayRange = new Vector2(0.5f, 2f);
    public Vector2 betweenBurstsRange = new Vector2(1f, 3f);
    public Vector2 durationRange = new Vector2(0.25f, 0.6f);
    public Vector2 intensityRange = new Vector2(0.15f, 0.35f);

    Vector3 _basePos;
    bool _running;
    Coroutine _routine;

    void Awake()
    {
        if (target == null && Camera.main != null) target = Camera.main.transform;
        if (target != null) _basePos = target.localPosition;
    }

    void OnEnable()
    {
        if (autoStart && _routine == null)
        {
            if (randomBursts) _routine = StartCoroutine(RandomLoop());
            else _routine = StartCoroutine(ShakeLoop(loop, defaultDuration, defaultIntensity));
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
        ResetPosition();
    }

    public void StartLoop()
    {
        loop = true;
        if (_routine == null) _routine = StartCoroutine(ShakeLoop(true, defaultDuration, defaultIntensity));
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
        ResetPosition();
    }

    public void TriggerOnce(float? duration = null, float? intensity = null)
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }
        _routine = StartCoroutine(ShakeOnce(duration ?? defaultDuration, intensity ?? defaultIntensity));
    }

    System.Collections.IEnumerator RandomLoop()
    {
        _running = true;
        float startDelay = Random.Range(initialDelayRange.x, initialDelayRange.y);
        if (startDelay > 0f) yield return Hold(startDelay);
        while (_running)
        {
            float dur = Random.Range(durationRange.x, durationRange.y);
            float amp = Random.Range(intensityRange.x, intensityRange.y);
            yield return ShakeOnce(dur, amp);
            float gap = Random.Range(betweenBurstsRange.x, betweenBurstsRange.y);
            if (gap > 0f) yield return Hold(gap);
        }
        _routine = null;
    }

    System.Collections.IEnumerator ShakeLoop(bool repeat, float dur, float amp)
    {
        _running = true;
        do
        {
            yield return ShakeOnce(dur, amp);
        }
        while (repeat && _running);
        _routine = null;
        _running = false;
    }

    System.Collections.IEnumerator ShakeOnce(float duration, float amplitude)
    {
        if (target == null || duration <= 0f || amplitude <= 0f) yield break;
        Vector3 origin = _basePos;
        float t = 0f;
        float seed = Random.value * 1000f;
        while (t < duration)
        {
            t += Delta();
            float progress = Mathf.Clamp01(t / duration);
            float damper = 1f - Mathf.Clamp01(2f * progress - 1f);
            float nx = (Mathf.PerlinNoise(seed, progress * frequency) - 0.5f) * 2f;
            float ny = (Mathf.PerlinNoise(seed + 10f, progress * frequency) - 0.5f) * 2f;
            Vector3 offset = new Vector3(nx, ny, 0f) * amplitude * damper;
            target.localPosition = origin + offset;
            yield return null;
        }
        ResetPosition();
    }

    void ResetPosition()
    {
        if (target != null) target.localPosition = _basePos;
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

    float Delta() => ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
}
