using System.Collections;
using UnityEngine;

/// <summary>
/// ทำให้แก้ว: แกว่งซ้าย‑ขวาเบา ๆ เท่านั้น
/// ใช้กับ GameObject TheGlass ใน PourBeerUI
/// </summary>
public class GlassShake : MonoBehaviour
{
    [Header("Sway Settings")]
    [Tooltip("ระยะการขยับซ้าย‑ขวา (world units)")]
    public float swayAmplitude = 0.2f;

    [Tooltip("ความเร็วในการแกว่ง (ยิ่งสูงยิ่งไป‑กลับเร็ว)")]
    public float swaySpeed = 1.5f;

    [Tooltip("เปิด/ปิดการขยับจากภายนอก (เช่น Shower ยังทำงานอยู่)")]
    public bool isShaking = true;

    Vector3 _basePosition;
    Quaternion _baseRotation;

    void Awake()
    {
        _basePosition = transform.position;
        _baseRotation = transform.rotation;
    }

    void OnEnable()
    {
        _basePosition = transform.position;
        _baseRotation = transform.rotation;
    }

    void OnDisable()
    {
        transform.position = _basePosition;
        transform.rotation = _baseRotation;
    }

    void Update()
    {
        if (!isShaking)
        {
            transform.position = Vector3.Lerp(transform.position, _basePosition, Time.deltaTime * 10f);
            transform.rotation = Quaternion.Slerp(transform.rotation, _baseRotation, Time.deltaTime * 10f);
            return;
        }

        float t = Time.time * swaySpeed;
        float swayX = Mathf.Sin(t) * swayAmplitude;
        Vector3 offset = new Vector3(swayX, 0f, 0f);
        transform.position = _basePosition + offset;
        transform.rotation = _baseRotation;
    }

    /// <summary>
    /// เรียกจากสคริปต์อื่นเพื่อเริ่ม/หยุดการขยับได้
    /// </summary>
    public void SetShaking(bool value)
    {
        isShaking = value;
    }
}
