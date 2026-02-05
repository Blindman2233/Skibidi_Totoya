using UnityEngine;

/// <summary>
/// ทำให้แก้วสั่นแบบสุ่มเป็นช่วง ๆ (สั่น‑หยุด‑สั่น‑หยุด)
/// ใช้กับ GameObject TheGlass ใน PourBeerUI
/// </summary>
public class GlassShake : MonoBehaviour
{
    [Header("Shake Settings")]
    [Tooltip("แรงสั่นสูงสุด (องศา)")]
    public float maxAngle = 4f;

    [Tooltip("ระยะการขยับตำแหน่งสูงสุด (world units)")]
    public float positionJitter = 0.05f;

    [Tooltip("ความเร็วในการเปลี่ยนค่า noise ยิ่งสูงยิ่งสั่นเร็ว")]
    public float shakeSpeed = 5f;

    [Header("Burst Settings")]
    [Tooltip("ระยะเวลาสั่นขั้นต่ำต่อหนึ่งรอบ (วินาที)")]
    public float minShakeDuration = 0.2f;

    [Tooltip("ระยะเวลาสั่นขั้นสูงต่อหนึ่งรอบ (วินาที)")]
    public float maxShakeDuration = 0.7f;

    [Tooltip("ระยะเวลาหยุดขั้นต่ำต่อหนึ่งรอบ (วินาที)")]
    public float minRestDuration = 0.15f;

    [Tooltip("ระยะเวลาหยุดขั้นสูงต่อหนึ่งรอบ (วินาที)")]
    public float maxRestDuration = 0.6f;

    [Tooltip("เปิด/ปิดการสั่นจากภายนอก (เช่น Shower ยังทำงานอยู่)")]
    public bool isShaking = true;

    Vector3 _basePosition;
    Quaternion _baseRotation;

    // ภายใน: ตอนนี้อยู่ในช่วงที่ "สั่น" อยู่หรือเปล่า
    bool _burstActive;
    Coroutine _shakeRoutine;

    void Awake()
    {
        _basePosition = transform.position;
        _baseRotation = transform.rotation;
    }

    void OnEnable()
    {
        // รีเซ็ตฐานทุกครั้งที่ UI ถูกเปิด
        _basePosition = transform.position;
        _baseRotation = transform.rotation;

        if (_shakeRoutine == null)
        {
            _shakeRoutine = StartCoroutine(ShakeLoop());
        }
    }

    void OnDisable()
    {
        if (_shakeRoutine != null)
        {
            StopCoroutine(_shakeRoutine);
            _shakeRoutine = null;
        }

        _burstActive = false;
        transform.position = _basePosition;
        transform.rotation = _baseRotation;
    }

    void Update()
    {
        if (!isShaking || !_burstActive)
        {
            // คืนค่ากลับสู่ฐานอย่างนุ่มนวลเมื่อหยุดสั่น
            transform.position = Vector3.Lerp(transform.position, _basePosition, Time.deltaTime * 10f);
            transform.rotation = Quaternion.Slerp(transform.rotation, _baseRotation, Time.deltaTime * 10f);
            return;
        }

        float t = Time.time * shakeSpeed;

        // ใช้ Perlin noise ให้การสั่นนุ่มกว่าสุ่มล้วน ๆ
        float noiseX = Mathf.PerlinNoise(t, 0.0f) * 2f - 1f;
        float noiseY = Mathf.PerlinNoise(0.0f, t) * 2f - 1f;
        float noiseAngle = Mathf.PerlinNoise(t, t * 0.37f) * 2f - 1f;

        Vector3 offset = new Vector3(noiseX, noiseY, 0f) * positionJitter;
        float angle = noiseAngle * maxAngle;

        transform.position = _basePosition + offset;
        transform.rotation = _baseRotation * Quaternion.Euler(0f, 0f, angle);
    }

    IEnumerator ShakeLoop()
    {
        while (true)
        {
            // ถ้าโดนสั่งปิดจากภายนอก ให้หยุดที่ฐานรอไว้
            if (!isShaking)
            {
                _burstActive = false;
                yield return null;
                continue;
            }

            // 1) พัก (ไม่สั่น)
            float rest = Random.Range(minRestDuration, maxRestDuration);
            _burstActive = false;
            float restTimer = 0f;
            while (restTimer < rest && isShaking)
            {
                restTimer += Time.deltaTime;
                yield return null;
            }

            if (!isShaking)
            {
                _burstActive = false;
                continue;
            }

            // 2) สั่น
            float shake = Random.Range(minShakeDuration, maxShakeDuration);
            _burstActive = true;
            float shakeTimer = 0f;
            while (shakeTimer < shake && isShaking)
            {
                shakeTimer += Time.deltaTime;
                yield return null;
            }

            _burstActive = false;
        }
    }

    /// <summary>
    /// เรียกจากสคริปต์อื่นเพื่อเริ่ม/หยุดการสั่นอย่างง่าย
    /// (เช่น Shower เรียกปิดเมื่อเลิกเทน้ำ)
    /// </summary>
    public void SetShaking(bool value)
    {
        isShaking = value;
        if (!value)
        {
            _burstActive = false;
        }
    }
}
