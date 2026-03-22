using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// แสดงปริมาณน้ำในแก้วตามค่า FillAmount/SpillAmount จาก PourLiquorManager
/// แนะนำให้ใช้กับ Image ที่อยู่ใน Canvas เดียวกับมินิเกม
/// </summary>
public class GlassFillUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Image ของน้ำในแก้ว (ต้องเปิด Fill Method เป็น Vertical)")]
    public Image glassFillImage;

    [Tooltip("Image หรือ UI แสดงน้ำที่หก (ไม่บังคับ)")]
    public Image spillFillImage;

    [Header("Display")]
    [Tooltip("จะใช้ค่าลงแก้ว (FillAmount01) โดยตรงเป็น fillAmount ของแก้ว")]
    public bool useRawFillAmount = true;

    [Tooltip("ถ้าไม่ใช้แบบดิบ จะคูณค่าลงแก้วด้วยสเกลนี้ (เผื่อแก้วกราฟิกสูงไม่เต็ม 100%)")]
    public float fillScale = 1f;

    private void Reset()
    {
        glassFillImage = GetComponent<Image>();
    }

    private void Update()
    {
        if (PourLiquorManager.Instance == null)
            return;

        // ไม่รันตอนมินิเกมไม่ได้ทำงาน เพื่อลดการอัพเดตไม่จำเป็น
        if (!PourLiquorManager.Instance.IsRunning)
            return;

        float fill = PourLiquorManager.Instance.FillAmount01;
        float spill = PourLiquorManager.Instance.SpillAmount01;

        if (!useRawFillAmount)
        {
            fill *= fillScale;
        }

        fill = Mathf.Clamp01(fill);

        if (glassFillImage != null)
        {
            glassFillImage.fillAmount = fill;
        }

        if (spillFillImage != null)
        {
            spillFillImage.fillAmount = Mathf.Clamp01(spill);
        }
    }
}

