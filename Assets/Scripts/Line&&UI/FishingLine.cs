using UnityEngine;

/// <summary>
/// 釣線：支援「直線」(節省) 與「垂墜曲線」(視覺) 雙模式。
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class FishingLine : MonoBehaviour
{
    [Header("曲線設定")]
    [Range(4, 64)] public int segments = 15;          // 線段數
    [Range(0f, 0.5f)] public float maxSagPercent = .5f; // 完全靜止時的下垂比例

    LineRenderer lr;
    Transform    endA, endB;

    // 當前是否啟用曲線
    bool useSag = false;
    Vector3[] pts;      // 快取頂點陣列
    float[]   velY;     // 垂向速度 (彈簧)

    const float K = 50f, DAMP = 5f; // 彈簧常數 / 阻尼

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 2;        // 起始 2 頂點
        pts  = new Vector3[segments + 1];
        velY = new float[segments + 1];
    }

    public void Show(bool on) => lr.enabled = on;

    public void SetTargets(Transform a, Transform b)
    {
        endA = a;
        endB = b;
    }

    /// <summary>切換是否要顯示垂墜曲線。</summary>
    public void EnableSag(bool on)
    {
        useSag = on;
        lr.positionCount = on ? segments + 1 : 2;
        // 重置頂點以避免殘影
        if (!on) lr.SetPosition(0, endA.position);
    }

    void LateUpdate()
    {
        if (!lr.enabled || !endA || !endB) return;

        if (!useSag)     // 直線模式
        {
            lr.SetPosition(0, endA.position);
            lr.SetPosition(1, endB.position);
            return;
        }

        // ── 曲線模式 ──
        Vector3 p0 = endA.position;
        Vector3 p2 = endB.position;

        // 中點下垂量
        float sag = maxSagPercent * Vector3.Distance(p0, p2);

        pts[0] = p0;
        pts[segments] = p2;

        for (int i = 1; i < segments; i++)
        {
            float t = i / (float)segments;
            // 理想目標位置 (二次 Bezier)
            Vector3 pos = Vector3.Lerp(p0, p2, t);
            pos.y -= sag * 4 * t * (1f - t);

            // 簡易彈簧 (只算 y，加點動態感)
            float diff = pts[i].y - pos.y;
            float acc  = (-K * diff) - (DAMP * velY[i]);
            velY[i] += acc * Time.deltaTime;
            pts[i].y += velY[i] * Time.deltaTime;

            // x/z 直接貼目標
            pts[i].x = pos.x;
            pts[i].z = pos.z;
        }
        lr.SetPositions(pts);
    }
}
