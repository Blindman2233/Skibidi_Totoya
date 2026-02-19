using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    public string targetSpriteName = "Beer";
    public GameObject winUI;
    public string winUIName = "WinUI";
    public bool pauseOnTrigger = true;
    bool triggered;

    public bool autoFix2D = true;
    public bool autoMatchBeerZ = true;
    public bool flattenZ = true;
    public float flattenZValue = 0f;

    void Awake()
    {
        if (autoFix2D)
        {
            var bc3d = GetComponent<BoxCollider>();
            if (bc3d != null) bc3d.enabled = false;
            var bc2d = GetComponent<BoxCollider2D>();
            if (bc2d == null) bc2d = gameObject.AddComponent<BoxCollider2D>();
            bc2d.isTrigger = true;
            var rb2d = GetComponent<Rigidbody2D>();
            if (rb2d == null) rb2d = gameObject.AddComponent<Rigidbody2D>();
            rb2d.bodyType = RigidbodyType2D.Kinematic;
            rb2d.simulated = true;
        }

        if (flattenZ)
        {
            var p = transform.position;
            p.z = flattenZValue;
            transform.position = p;
        }

        if (autoMatchBeerZ)
        {
            var beers = Object.FindObjectsOfType<SpriteRenderer>();
            foreach (var sr in beers)
            {
                if (sr.sprite != null && sr.sprite.name == targetSpriteName)
                {
                    var p = transform.position;
                    p.z = sr.transform.position.z;
                    transform.position = p;
                    break;
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        var sr = other.GetComponent<SpriteRenderer>();
        if (sr == null) return;
        var nameMatch = (sr.sprite != null && sr.sprite.name == targetSpriteName) || other.gameObject.name == targetSpriteName;
        if (!nameMatch) return;
        var ui = winUI != null ? winUI : GameObject.Find(winUIName);
        if (ui == null) return;
        triggered = true;
        ui.SetActive(true);
        if (pauseOnTrigger) Time.timeScale = 0f;
    }
}
