using UnityEngine;

public class BeerGlass : MonoBehaviour
{
    [Header("Beer Visuals")]
    // ลากออบเจกต์ Sprite สีเหลืองของคุณมาใส่ช่องนี้ใน Inspector
    public Transform beerSpriteTransform;
    public string sortingLayerName = "Default";
    public int sortingOrder = 100;
    public int beerLayer = 0;

    [Header("Fill Settings")]
    public float fillSpeed = 0.5f; // ความเร็วในการเติมเบียร์
    public float maxScaleY = 1.0f; // ความสูงสูงสุดของเบียร์ตอนเต็มแก้ว (ปรับตามสเกลจริงของคุณ)

    void Start()
    {
        // เริ่มต้นเกมมา บังคับให้เบียร์มีขนาด Y เป็น 0 (แก้วเปล่า)
        if (beerSpriteTransform != null)
        {
            var sr = beerSpriteTransform.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingLayerName = sortingLayerName;
                sr.sortingOrder = sortingOrder;
            }
            beerSpriteTransform.gameObject.layer = beerLayer;
            var pos = beerSpriteTransform.localPosition;
            pos.z = 0f;
            beerSpriteTransform.localPosition = pos;
            Vector3 startScale = beerSpriteTransform.localScale;
            startScale.y = 0f;
            beerSpriteTransform.localScale = startScale;
        }
    }

    // ฟังก์ชันนี้จะถูกเรียกจากสคริปต์ NewWater เวลาน้ำไหลมาโดน
    public void ReceiveWater(float deltaTime)
    {
        if (beerSpriteTransform.localScale.y < maxScaleY)
        {
            Vector3 currentScale = beerSpriteTransform.localScale;
            // เพิ่มความสูงขึ้นเรื่อยๆ ตามเวลาและความเร็ว
            currentScale.y += deltaTime * fillSpeed;

            // ล็อคค่าไม่ให้ล้นเกินแก้ว
            currentScale.y = Mathf.Clamp(currentScale.y, 0, maxScaleY);

            beerSpriteTransform.localScale = currentScale;
        }
    }
}
