using System.Collections.Generic;
using UnityEngine;

using list = System.Collections.Generic.List<Particle>;
using vector2 = UnityEngine.Vector2;

/// <summary>
/// 2D SPH‑style fluid simulation driven by particles.
/// ทำหน้าที่:
/// - รวบรวม particle ลูกทั้งหมดใต้ GameObject นี้
/// - แบ่งพื้นที่เป็นกริดสำหรับหาพวกเพื่อนบ้าน
/// - คำนวณ density, pressure, viscosity และอัปเดตสถานะของทุก particle
/// </summary>
public class Simulation : MonoBehaviour
{
    // ------------------------------------------------------------------
    // Config values (อ่านอย่างเดียวจาก Config)
    // ------------------------------------------------------------------

    public static readonly int N = Config.N;
    public static readonly float SIM_W = Config.SIM_W;
    public static readonly float BOTTOM = Config.BOTTOM;
    public static readonly float DAM = Config.DAM;
    public static readonly int DAM_BREAK = Config.DAM_BREAK;
    public static readonly float G = Config.G;
    public static readonly float SPACING = Config.SPACING;
    public static readonly float K = Config.K;
    public static readonly float K_NEAR = Config.K_NEAR;
    public static readonly float REST_DENSITY = Config.REST_DENSITY;
    public static readonly float R = Config.R;
    public static readonly float SIGMA = Config.SIGMA;
    public static readonly float MAX_VEL = Config.MAX_VEL;
    public static readonly float WALL_DAMP = Config.WALL_DAMP;
    public static readonly float VEL_DAMP = Config.VEL_DAMP;
    public static readonly float DT = Config.DT;
    public static readonly float WALL_POS = Config.WALL_POS;

    // ------------------------------------------------------------------
    // Simulation state
    // ------------------------------------------------------------------

    /// <summary>
    /// รายชื่อ particle ทั้งหมด (อัปเดตใหม่ทุกเฟรมจากลูกของ GameObject นี้)
    /// </summary>
    public list particles = new list();

    /// <summary>
    /// Prefab / ตัวอย่าง particle ไว้ใช้สร้างเพิ่มจากที่อื่น
    /// </summary>
    public GameObject baseParticle;

    // Spatial partitioning grid
    [Header("Grid Settings")]
    [Tooltip("จำนวนช่องกริดแกน X ใช้สำหรับค้นหาเพื่อนบ้าน")]
    public int gridSizeX = 60;

    [Tooltip("จำนวนช่องกริดแกน Y ใช้สำหรับค้นหาเพื่อนบ้าน")]
    public int gridSizeY = 30;

    [Tooltip("ขอบซ้ายของพื้นที่จำลองใน world space")]
    public float xMin = 1.8f;

    [Tooltip("ขอบขวาของพื้นที่จำลองใน world space")]
    public float xMax = 6.4f;

    [Tooltip("ขอบล่างของพื้นที่จำลองใน world space")]
    public float yMin = -1.4f;

    [Tooltip("ขอบบนของพื้นที่จำลองใน world space")]
    public float yMax = 0.61f;

    // กริดเก็บรายการเพื่อนบ้าน (allocate ครั้งเดียว)
    private list[,] _grid;

    // ------------------------------------------------------------------
    // Working variables (reuse ทุกเฟรม เพื่อลด GC)
    // ------------------------------------------------------------------

    float _dist;
    float _distance;
    float _normalDistance;
    float _relativeDistance;
    float _totalPressure;
    float _velocityDifference;
    vector2 _pressureForce;
    vector2 _particleToNeighbor;
    vector2 _pressureVector;
    vector2 _normalPToN;
    vector2 _viscosityForce;

    void Awake()
    {
        // หา base particle เผื่อมีการอ้างอิงจาก scene เดิม
        if (baseParticle == null)
        {
            var found = GameObject.Find("Base_Particle");
            if (found != null)
            {
                baseParticle = found;
            }
        }

        // เตรียมกริดสำหรับ spatial hashing (allocate ครั้งเดียว)
        _grid = new list[gridSizeX, gridSizeY];
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                _grid[x, y] = new list();
            }
        }
    }

    void Update()
    {
        // 1) รวบรวม particle ลูกทั้งหมด
        RefreshParticleList();

        // 2) สร้าง spatial grid สำหรับเพื่อนบ้าน
        RebuildGrid();

        // 3) อัปเดตสถานะพื้นฐาน (แรงโน้มถ่วง ฯลฯ)
        foreach (Particle p in particles)
        {
            p.UpdateState();
        }

        // 4) คำนวณ density แล้ว pressure และ viscosity
        CalculateDensity(particles);

        foreach (Particle p in particles)
        {
            p.CalculatePressure();
        }

        CreatePressure(particles);
        CalculateViscosity(particles);
    }

    // ------------------------------------------------------------------
    // ขั้นตอนหลักของ simulation
    // ------------------------------------------------------------------

    void RefreshParticleList()
    {
        particles.Clear();

        foreach (Transform child in transform)
        {
            Particle p = child.GetComponent<Particle>();
            if (p != null)
            {
                particles.Add(p);
            }
        }
    }

    void RebuildGrid()
    {
        // ล้างกริดเก่า
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                _grid[x, y].Clear();
            }
        }

        // ใส่ particle ลง cell ตามตำแหน่ง
        float invWidth = 1f / (xMax - xMin);
        float invHeight = 1f / (yMax - yMin);

        foreach (Particle p in particles)
        {
            int gx = (int)((p.pos.x - xMin) * invWidth * gridSizeX);
            int gy = (int)((p.pos.y - yMin) * invHeight * gridSizeY);

            p.grid_x = gx;
            p.grid_y = gy;

            if (gx >= 0 && gx < gridSizeX && gy >= 0 && gy < gridSizeY)
            {
                _grid[gx, gy].Add(p);
            }
        }
    }

    /// <summary>
    /// คำนวณ density / near density และกรอก neighbours ให้แต่ละ particle
    /// </summary>
    void CalculateDensity(list particlesList)
    {
        foreach (Particle p in particlesList)
        {
            // clear ค่าเก่าออกก่อน (กันสะสมข้ามเฟรม)
            p.rho = 0f;
            p.rho_near = 0f;
            p.neighbours.Clear();

            // ตรวจ 9 cell รอบตัว
            for (int gx = p.grid_x - 1; gx <= p.grid_x + 1; gx++)
            {
                for (int gy = p.grid_y - 1; gy <= p.grid_y + 1; gy++)
                {
                    if (gx < 0 || gx >= gridSizeX || gy < 0 || gy >= gridSizeY)
                        continue;

                    foreach (Particle n in _grid[gx, gy])
                    {
                        _dist = Vector2.Distance(p.pos, n.pos);

                        if (_dist < R)
                        {
                            _normalDistance = 1f - _dist / R;

                            float nd2 = _normalDistance * _normalDistance;
                            float nd3 = nd2 * _normalDistance;

                            p.rho += nd2;
                            p.rho_near += nd3;

                            n.rho += nd2;
                            n.rho_near += nd3;

                            if (!p.neighbours.Contains(n))
                            {
                                p.neighbours.Add(n);
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// สร้างแรงกด (pressure) ระหว่างเพื่อนบ้านแต่ละคู่
    /// </summary>
    void CreatePressure(list particlesList)
    {
        foreach (Particle p in particlesList)
        {
            _pressureForce = vector2.zero;

            foreach (Particle n in p.neighbours)
            {
                _particleToNeighbor = n.pos - p.pos;
                _distance = _particleToNeighbor.magnitude;

                if (_distance <= 0f)
                    continue;

                _normalDistance = 1f - _distance / R;

                float nd2 = _normalDistance * _normalDistance;
                float nd3 = nd2 * _normalDistance;

                _totalPressure =
                    (p.press + n.press) * nd2 +
                    (p.press_near + n.press_near) * nd3;

                _pressureVector = _totalPressure * (_particleToNeighbor / _distance);

                n.force += _pressureVector;
                _pressureForce += _pressureVector;
            }

            p.force -= _pressureForce;
        }
    }

    /// <summary>
    /// คำนวณแรงหนืด (viscosity) ระหว่างเพื่อนบ้าน
    /// </summary>
    void CalculateViscosity(list particlesList)
    {
        foreach (Particle p in particlesList)
        {
            foreach (Particle n in p.neighbours)
            {
                _particleToNeighbor = n.pos - p.pos;
                _distance = _particleToNeighbor.magnitude;

                if (_distance <= 0f)
                    continue;

                _normalPToN = _particleToNeighbor / _distance;
                _relativeDistance = _distance / R;

                _velocityDifference = Vector2.Dot(p.vel - n.vel, _normalPToN);
                if (_velocityDifference > 0f)
                {
                    _viscosityForce =
                        (1f - _relativeDistance) *
                        _velocityDifference *
                        SIGMA *
                        _normalPToN;

                    // แบ่งแรงครึ่ง ๆ ให้สองฝั่ง
                    p.vel -= _viscosityForce * 0.5f;
                    n.vel += _viscosityForce * 0.5f;
                }
            }
        }
    }
}
