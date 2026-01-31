using UnityEngine;

public class DialogueAudioHandler : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip[] voiceSamples; // ใส่ไฟล์เสียงสั้นๆ หลายๆ แบบได้ที่นี่

    [Range(0.1f, 0.5f)]
    public float pitchRange = 0.2f; // ขอบเขตการสุ่มความสูงต่ำของเสียง

    private float basePitch;

    void Start()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        basePitch = audioSource.pitch;
    }

    // ฟังก์ชันนี้ให้เรียกใช้ทุกครั้งที่ตัวอักษรพิมพ์ออกมา (เช่น ใน Coroutine ของ Text Display)
    public void PlayVoiceSound()
    {
        if (voiceSamples.Length == 0) return;

        // 1. สุ่มเลือกไฟล์เสียงจาก List
        int randomIndex = Random.Range(0, voiceSamples.Length);
        audioSource.clip = voiceSamples[randomIndex];

        // 2. สุ่ม Pitch เพื่อให้เสียงมีไดนามิก (แบบ Sans หรือ Papyrus)
        // สูตรคือ basePitch +/- ค่า random เล็กน้อย
        audioSource.pitch = basePitch + Random.Range(-pitchRange, pitchRange);

        // 3. เล่นเสียง
        audioSource.PlayOneShot(audioSource.clip);
    }
}