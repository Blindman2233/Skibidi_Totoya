using UnityEngine;

public class BeerTrigger : MonoBehaviour
{
    public LineRenderer targetLine;
    public float maxWidth = 1f;
    public float addWidthPerHit = 0.05f;
    public float widthChangeSpeed = 6f;
    public string liquidTag = "LiquidParticle";
    float _targetWidth;

    void Update()
    {
        if (targetLine != null)
        {
            targetLine.widthMultiplier = Mathf.MoveTowards(targetLine.widthMultiplier, _targetWidth, Time.deltaTime * widthChangeSpeed);
        }
    }

    void Awake()
    {
        if (targetLine == null) targetLine = GetComponent<LineRenderer>();
        _targetWidth = targetLine != null ? targetLine.widthMultiplier : 0f;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsLiquid(other)) return;
        if (targetLine == null) return;
        _targetWidth = Mathf.Min(maxWidth, targetLine.widthMultiplier + addWidthPerHit);
    }

    bool IsLiquid(Collider2D other)
    {
        if (other == null) return false;
        if (other.GetComponent<Particle>() != null) return true;
        if (!string.IsNullOrEmpty(liquidTag) && other.tag == liquidTag) return true;
        return false;
    }
}
