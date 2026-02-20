using UnityEngine;
using System.Collections.Generic;

public class PouringSystem : MonoBehaviour
{
    public Transform pourPoint;      // จุดที่น้ำออกจากขวด
    public LineRenderer lineRenderer;
    public EdgeCollider2D edgeCollider;

    private void Update()
    {
        // ตรวจสอบว่าเหยือกเอียงจนถึงจุดที่น้ำควรไหลหรือไม่
        if (transform.eulerAngles.z > 45 && transform.eulerAngles.z < 150)
        {
            StartPouring();
        }
        else
        {
            StopPouring();
        }
    }

    void StartPouring()
    {
        lineRenderer.enabled = true;

        // จุดเริ่มต้นคือปากขวด
        Vector3 startPos = pourPoint.position;
        // คำนวณจุดตก (Raycast ลงไปข้างล่าง)
        RaycastHit2D hit = Physics2D.Raycast(startPos, Vector2.down);

        Vector3 endPos;
        if (hit.collider != null)
        {
            endPos = hit.point; // ถ้าน้ำกระทบพื้นหรือแก้ว
        }
        else
        {
            endPos = startPos + Vector3.down * 10f; // ถ้าไม่โดนอะไรเลยให้ยาวลงไป 10 หน่วย
        }

        // อัปเดต Line Renderer
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);

        // อัปเดต Edge Collider ให้ตรงกับสายน้ำเพื่อให้ตรวจจับการเติมน้ำในแก้วได้
        List<Vector2> points = new List<Vector2>();
        points.Add(lineRenderer.transform.InverseTransformPoint(startPos));
        points.Add(lineRenderer.transform.InverseTransformPoint(endPos));
        edgeCollider.SetPoints(points);
    }

    void StopPouring()
    {
        lineRenderer.enabled = false;
        edgeCollider.enabled = false;
    }
}