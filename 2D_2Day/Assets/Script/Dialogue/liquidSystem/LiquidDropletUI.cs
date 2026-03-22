using UnityEngine;

/// <summary>
/// เคลื่อนที่หยดน้ำแบบ UI (Screen Space) ลงด้านล่าง แล้วทำลายตัวเองเมื่อหมดเวลา
/// ใช้กับ Prefab น้ำที่เป็น UI (เช่น Image) ภายใต้ Canvas
/// </summary>
public class LiquidDropletUI : MonoBehaviour
{
    [Tooltip("ความเร็วตก (พิกเซล/วินาที)")]
    public float fallSpeed = 650f;

    [Tooltip("อายุของหยดน้ำก่อนทำลาย (วินาที)")]
    public float lifeTime = 1.2f;

    private RectTransform rectTransform;
    private float remaining;

    private void Awake()
    {
        rectTransform = transform as RectTransform;
        remaining = lifeTime;
    }

    private void OnEnable()
    {
        remaining = lifeTime;
    }

    private void Update()
    {
        remaining -= Time.deltaTime;
        if (remaining <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        if (rectTransform != null)
        {
            rectTransform.anchoredPosition += Vector2.down * (fallSpeed * Time.deltaTime);
        }
        else
        {
            transform.position += Vector3.down * (fallSpeed * Time.deltaTime);
        }
    }
}

