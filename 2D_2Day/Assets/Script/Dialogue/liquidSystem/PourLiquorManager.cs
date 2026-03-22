using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum PourLiquorResult
{
    None,
    Success,
    Fail
}

public class PourLiquorManager : MonoBehaviour
{
    public static PourLiquorManager Instance;

    [Header("UI Root")]
    [Tooltip("Canvas / Panel หลักของมินิเกมรินเหล้า")]
    public GameObject pourLiquorUI;

    [Header("Bottle & Glass UI")]
    [Tooltip("รูปขวดที่จะขยับตามเมาส์")]
    public Image bottleImage;

    [Tooltip("พื้นที่ของแก้ว ใช้เช็คว่าเมาส์/ขวดอยู่เหนือแก้วหรือไม่")]
    public RectTransform glassArea;

    [Header("Pour Input")]
    [Tooltip("ปุ่มสำหรับเทน้ำ/เหล้า (กดค้างเพื่อให้ไหล)")]
    public KeyCode pourKey = KeyCode.Space;

    [Tooltip("ยังให้คลิกเมาส์ซ้ายเพื่อเทได้ด้วย (เผื่อ debug/ทางเลือก)")]
    public bool allowMouseButtonPour = false;

    [Header("Liquid Visual (Prefab)")]
    [Tooltip("Prefab น้ำ/เหล้าที่จะปล่อยออกจากปากขวด (แนะนำเป็น UI Image + LiquidDropletUI)")]
    public GameObject liquidPrefab;

    [Tooltip("จุดปล่อยน้ำ (ถ้าไม่ใส่ จะใช้ตำแหน่ง bottleImage แทน)")]
    public RectTransform liquidSpawnPoint;

    [Tooltip("จำนวนหยด/วินาที")]
    public float dropletRate = 20f;

    [Tooltip("สุ่มกระจายตำแหน่งเล็กน้อย (พิกเซล)")]
    public float dropletSpawnJitter = 8f;

    [Header("Progress UI (ไม่บังคับ)")]
    public Image fillProgressBar;
    public Image spillProgressBar;
    public TMP_Text timerText;

    private PourLiquorSettings currentSettings;
    private EventData currentEvent;
    private Action<PourLiquorResult> onFinishedCallback;
    private bool startedFromDialogue;

    private float currentFillAmount;   // 0-1
    private float currentSpillAmount;  // 0-1
    private float remainingTime;
    private bool isRunning;
    private float dropletAccumulator;

    public float FillAmount01 => currentFillAmount;
    public float SpillAmount01 => currentSpillAmount;
    public bool IsRunning => isRunning;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (pourLiquorUI != null)
        {
            pourLiquorUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (!isRunning || currentSettings == null)
            return;

        HandleBottleFollowMouse();
        HandlePourLogic();
        HandleTimer();
        UpdateUI();
    }

    public void StartPourLiquor(EventData eventData)
    {
        if (eventData == null || eventData.pourLiquorSettings == null)
        {
            Debug.LogError("PourLiquorManager: EventData หรือ PourLiquorSettings ว่างอยู่ ไม่สามารถเริ่มมินิเกมได้");
            return;
        }

        startedFromDialogue = false;
        onFinishedCallback = null;

        currentEvent = eventData;
        currentSettings = eventData.pourLiquorSettings;

        StartCore();
    }

    public void StartPourLiquorFromDialogue(PourLiquorSettings settings, Action<PourLiquorResult> callback)
    {
        if (settings == null)
        {
            Debug.LogError("PourLiquorManager: PourLiquorSettings จาก Dialogue ว่างอยู่ ไม่สามารถเริ่มมินิเกมได้");
            callback?.Invoke(PourLiquorResult.Fail);
            return;
        }

        startedFromDialogue = true;
        onFinishedCallback = callback;

        currentEvent = null;
        currentSettings = settings;

        StartCore();
    }

    private void StartCore()
    {
        currentFillAmount = 0f;
        currentSpillAmount = 0f;
        remainingTime = currentSettings.timeLimit;
        isRunning = true;
        dropletAccumulator = 0f;

        if (pourLiquorUI != null)
        {
            pourLiquorUI.SetActive(true);
        }
    }

    private void EndPourLiquor(PourLiquorResult result)
    {
        isRunning = false;
        dropletAccumulator = 0f;

        if (pourLiquorUI != null)
        {
            pourLiquorUI.SetActive(false);
        }

        if (startedFromDialogue)
        {
            var callback = onFinishedCallback;

            startedFromDialogue = false;
            onFinishedCallback = null;
            currentSettings = null;
            currentEvent = null;

            callback?.Invoke(result);
            return;
        }

        // เลือก Event ถัดไปจาก nextEvents: index 0 = ชนะ, index 1 = แพ้
        EventData next = null;
        if (currentEvent != null && currentEvent.nextEvents != null && currentEvent.nextEvents.Count > 0)
        {
            if (result == PourLiquorResult.Success && currentEvent.nextEvents.Count > 0)
            {
                next = currentEvent.nextEvents[0];
            }
            else if (result == PourLiquorResult.Fail && currentEvent.nextEvents.Count > 1)
            {
                next = currentEvent.nextEvents[1];
            }
        }

        currentSettings = null;
        EventData finishedEvent = currentEvent;
        currentEvent = null;

        if (next != null && EventManager.Instance != null)
        {
            EventManager.Instance.TriggerEvent(next);
        }
        else
        {
            // ถ้าไม่ได้ตั้ง nextEvent ไว้ ก็ให้ระบบ Event ทำงานต่อปกติ
            if (EventManager.Instance != null && finishedEvent != null)
            {
                // ให้ EventManager ตรวจ Event ใหม่จาก state ปัจจุบัน (ผ่าน TriggerEvent ของ next อยู่แล้วในเคสปกติ)
                // ที่นี่ไม่ต้องทำอะไรเพิ่มเติมมาก
            }
        }
    }

    private void HandleBottleFollowMouse()
    {
        if (bottleImage == null)
            return;

        // สมมุติว่าใช้ Canvas แบบ Screen Space - Overlay
        bottleImage.rectTransform.position = Input.mousePosition;
    }

    private void HandlePourLogic()
    {
        bool isPouring = Input.GetKey(pourKey);
        if (!isPouring && allowMouseButtonPour)
        {
            isPouring = Input.GetMouseButton(0);
        }

        if (!isPouring)
            return;

        if (glassArea == null || currentSettings == null)
            return;

        bool isOverGlass = RectTransformUtility.RectangleContainsScreenPoint(glassArea, Input.mousePosition);
        float delta = Time.deltaTime;

        // สร้างหยดน้ำและตรวจสอบว่าจริงๆ แล้วตกในแก้วหรือไม่
        SpawnLiquidVisual(delta, isOverGlass);

        if (isOverGlass)
        {
            currentFillAmount += currentSettings.pourSpeed * delta;
        }
        else
        {
            currentSpillAmount += currentSettings.spillSpeed * delta;
        }

        currentFillAmount = Mathf.Clamp01(currentFillAmount);
        currentSpillAmount = Mathf.Clamp01(currentSpillAmount);

        // ไม่ให้ SpillAmount ไปลด FillAmount แต่ยังจำกัดค่ารวมไม่ให้เกิน 1
        // แต่ FillAmount จะเต็มได้ถึง 1 ไม่ว่าจะมี Spill เท่าไหร่
        if (currentFillAmount > 1f)
        {
            currentFillAmount = 1f;
        }

        // ถ้า FillAmount เต็มแล้ว ให้จบเกมทันที
        if (currentFillAmount >= 1f)
        {
            EvaluateResult();
        }
    }

    private void SpawnLiquidVisual(float deltaTime, bool isOverGlass)
    {
        if (liquidPrefab == null || dropletRate <= 0f)
            return;

        dropletAccumulator += dropletRate * deltaTime;
        int spawnCount = Mathf.FloorToInt(dropletAccumulator);
        if (spawnCount <= 0)
            return;

        dropletAccumulator -= spawnCount;

        var spawn = liquidSpawnPoint != null ? liquidSpawnPoint : (bottleImage != null ? bottleImage.rectTransform : null);
        if (spawn == null)
            return;

        Transform parent = pourLiquorUI != null ? pourLiquorUI.transform : null;

        for (int i = 0; i < spawnCount; i++)
        {
            GameObject droplet = Instantiate(liquidPrefab, parent);

            // วางแบบ Screen Space UI
            Vector3 pos = spawn.position;
            float j = dropletSpawnJitter;
            if (j > 0f)
            {
                pos.x += UnityEngine.Random.Range(-j, j);
                pos.y += UnityEngine.Random.Range(-j, j);
            }
            droplet.transform.position = pos;

            // เพิ่ม component หรือตั้งค่าให้หยดน้ำรู้ว่าตกในแก้วหรือไม่ (สำหรับอนาคต)
            var dropletUI = droplet.GetComponent<LiquidDropletUI>();
            if (dropletUI != null)
            {
                // สามารถเพิ่ม logic ให้หยดน้ำมีพฤติกรรมต่างกันได้
                // เช่น ถ้าตกในแก้วอาจจะหายเร็วกว่า หรือมี effect พิเศษ
            }
        }
    }

    private void HandleTimer()
    {
        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            EvaluateResult();
        }
    }

    private void EvaluateResult()
    {
        if (currentSettings == null)
        {
            EndPourLiquor(PourLiquorResult.None);
            return;
        }

        bool enoughInGlass = currentFillAmount >= currentSettings.targetFillPercent;
        bool tooMuchSpill = currentSpillAmount > currentSettings.maxSpillPercent;

        PourLiquorResult result;
        if (enoughInGlass && !tooMuchSpill)
        {
            result = PourLiquorResult.Success;
        }
        else
        {
            result = PourLiquorResult.Fail;
        }

        EndPourLiquor(result);
    }

    private void UpdateUI()
    {
        if (fillProgressBar != null)
        {
            fillProgressBar.fillAmount = currentFillAmount;
        }

        if (spillProgressBar != null)
        {
            spillProgressBar.fillAmount = currentSpillAmount;
        }

        if (timerText != null && currentSettings != null)
        {
            timerText.text = Mathf.CeilToInt(remainingTime).ToString();
        }

        Debug.Log($"Fill: {currentFillAmount:F2} | Spill: {currentSpillAmount:F2} | Target: {currentSettings.targetFillPercent}");
    }
}

