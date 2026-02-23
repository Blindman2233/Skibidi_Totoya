using UnityEngine;

public class CamShakeEfx : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("ลาก Main Camera มาใส่ที่นี่ (ถ้าไม่ใส่จะใช้ Camera.main หรือ Object นี้)")]
    public Transform targetCamera;

    [Header("Shake Settings")]
    [Tooltip("ความแรงในการเขย่าปกติ (Normal Intensity)")]
    public float shakeAmount = 0.1f;
    
    [Tooltip("ความเร็วในการเขย่า (Speed)")]
    public float shakeSpeed = 2.0f;

    [Header("Random Jolt (กระตุกแรง)")]
    [Tooltip("เปิดใช้งานการกระตุกแรงแบบสุ่ม")]
    public bool enableRandomJolts = false;

    [Tooltip("ความแรงตอนกระตุก (Jolt Intensity)")]
    public float joltForce = 1.0f;

    [Tooltip("ระยะเวลาที่กระตุก (Duration in seconds)")]
    public float joltDuration = 0.2f;

    [Tooltip("เวลาน้อยที่สุดที่จะเกิดการกระตุกครั้งต่อไป")]
    public float minJoltInterval = 2.0f;

    [Tooltip("เวลามากที่สุดที่จะเกิดการกระตุกครั้งต่อไป")]
    public float maxJoltInterval = 5.0f;

    private Vector3 initialPosition;
    private float timeOffset;
    
    // Jolt variables
    private float nextJoltTime;
    private bool isJolting;
    private float joltEndTime;

    void Start()
    {
        // ถ้าไม่ได้กำหนด targetCamera ให้ลองหาอัตโนมัติ
        if (targetCamera == null)
        {
            if (GetComponent<Camera>() != null) targetCamera = transform;
            else if (Camera.main != null) targetCamera = Camera.main.transform;
            else targetCamera = transform;
        }

        // เก็บตำแหน่งเริ่มต้นของกล้อง (Local Position)
        if (targetCamera != null)
        {
            initialPosition = targetCamera.localPosition;
        }
        
        // สุ่มค่าเริ่มต้นเพื่อให้การเขย่าไม่เหมือนกันทุกครั้ง
        timeOffset = Random.Range(0f, 100f);

        // กำหนดเวลาที่จะกระตุกครั้งแรก
        ScheduleNextJolt();
    }

    void Update()
    {
        if (targetCamera == null) return;

        float currentShakeX = 0f;
        float currentShakeY = 0f;
        float currentMagnitude = shakeAmount;

        // Check logic การกระตุก (Jolt)
        if (enableRandomJolts)
        {
            // ถ้าถึงเวลาที่จะกระตุก และยังไม่กระตุกอยู่
            if (!isJolting && Time.time >= nextJoltTime)
            {
                isJolting = true;
                joltEndTime = Time.time + joltDuration;
            }

            // ถ้ากำลังกระตุกอยู่
            if (isJolting)
            {
                // ใช้ Random.Range แบบดิบๆ เพื่อให้ดู "กระตุก" (Jagged/Glitchy) ไม่นุ่มนวล
                currentShakeX = Random.Range(-1f, 1f);
                currentShakeY = Random.Range(-1f, 1f);
                currentMagnitude = joltForce;

                // ถ้าหมดเวลากระตุกแล้ว
                if (Time.time >= joltEndTime)
                {
                    isJolting = false;
                    ScheduleNextJolt();
                }
            }
            else
            {
                // ถ้าไม่กระตุก ใช้ Perlin Noise แบบนุ่มนวล
                currentShakeX = Mathf.PerlinNoise(Time.time * shakeSpeed + timeOffset, 0) * 2 - 1;
                currentShakeY = Mathf.PerlinNoise(0, Time.time * shakeSpeed + timeOffset) * 2 - 1;
            }
        }
        else
        {
            // ถ้าปิดโหมดกระตุก ก็ใช้ Perlin Noise ปกติ
            currentShakeX = Mathf.PerlinNoise(Time.time * shakeSpeed + timeOffset, 0) * 2 - 1;
            currentShakeY = Mathf.PerlinNoise(0, Time.time * shakeSpeed + timeOffset) * 2 - 1;
        }

        // คำนวณตำแหน่งใหม่
        Vector3 newPos = initialPosition + new Vector3(currentShakeX, currentShakeY, 0) * currentMagnitude;

        // อัปเดตตำแหน่งกล้อง
        targetCamera.localPosition = newPos;
    }

    void ScheduleNextJolt()
    {
        nextJoltTime = Time.time + Random.Range(minJoltInterval, maxJoltInterval);
    }

    // ฟังก์ชันสำหรับรีเซ็ตตำแหน่ง
    public void StopShake()
    {
        shakeAmount = 0f;
        isJolting = false;
        if (targetCamera != null)
        {
            targetCamera.localPosition = initialPosition;
        }
    }
    
    // ฟังก์ชันเรียกใช้ Jolt ทันที (เผื่ออยากเรียกจาก Event อื่น)
    public void TriggerJolt()
    {
        isJolting = true;
        joltEndTime = Time.time + joltDuration;
    }
}
