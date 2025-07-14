// SimpleRodController.cs  (拋竿 = 弧線 Rigidbody / 收竿 = Destroy)
// -------------------------------------------------------------
// 功能：
//   • 點 CastButton  → 在 RodTip 生成魚標 (Prefab) 並以拋物線飛向 TargetPos
//   • 期間用 LineRenderer 連接 RodTip ↔ 魚標
//   • 任何滑鼠左鍵 → 收竿：摧毀魚標 & 釣線，回到 Idle
// -------------------------------------------------------------

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(FishingLine))] // 讓同物件自帶 LineRenderer 連線腳本
public class SimpleRodController : MonoBehaviour
{
    /*──────── Inspector 參數 ────────*/
    [Header("UI")]
    [SerializeField] private Button castButton;

    [Header("Transforms")]
    [SerializeField] private Transform rodTip;        // 釣竿尖端 (拋出位置)
    [SerializeField] private Transform targetPos;     // 魚標落點 (空物件)

    [Header("Prefabs")]
    [SerializeField] private GameObject bobberPrefab; // 魚標預製 (帶 Rigidbody)

    [Header("Physics")]
    [SerializeField] private float launchAngleDeg = 45f; // 拋射角
    [SerializeField] private float gravity       = 9.81f; // y↓ 正

    /*──────── 私有變數 ─────────────*/
    private enum LocalState { Idle, Casting, WaitingForReel, Reeling }
    private LocalState state = LocalState.Idle;

    private GameObject  currentBobber;  // 目前生成的魚標
    private FishingLine line;           // 連線腳本

    private void Awake()
    {
        castButton.onClick.AddListener(OnCastClicked);
        line = GetComponent<FishingLine>();
        line.enabled = false; // 沒有魚標時隱藏連線
    }

    private void Update()
    {
        if (state == LocalState.WaitingForReel && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(ReelRoutine());
        }
    }

    /*──────────────── Cast ▸ 拋竿 ───────────────*/
    private void OnCastClicked()
    {
        if (state != LocalState.Idle) return;
        if (bobberPrefab == null || rodTip == null || targetPos == null) return;
        StartCoroutine(CastRoutine());
    }

    private IEnumerator CastRoutine()
    {
        state = LocalState.Casting;

        // 1) 生成魚標於 RodTip 位置
        currentBobber = Instantiate(bobberPrefab, rodTip.position, Quaternion.identity);
        Rigidbody2D rb = currentBobber.GetComponent<Rigidbody2D>();
        if (rb == null) rb = currentBobber.AddComponent<Rigidbody2D>();
        rb.gravityScale = 1f;

        // 2) 計算拋射初速度 (簡式) --------------------------------------------------
        Vector2 start = rodTip.position;
        Vector2 end   = targetPos.position;
        float angle   = launchAngleDeg * Mathf.Deg2Rad;
        float dist    = Vector2.Distance(start, end);

        // v^2 = d * g / sin(2θ)
        float vInit   = Mathf.Sqrt(dist * gravity / Mathf.Sin(2 * angle));
        Vector2 dir   = (end - start).normalized;
        // 調整方向至指定角度 (將方向水平化再旋轉)
        Vector2 v0 = new Vector2(dir.x, 0).normalized * vInit * Mathf.Cos(angle) + Vector2.up * vInit * Mathf.Sin(angle);
        rb.velocity = v0;

        // 3) 啟用釣線
        line.SetTargets(rodTip, currentBobber.transform);
        line.enabled = true;

        // 4) 進入等待收竿
        yield return new WaitForSeconds(0.5f); // 可視作拋竿動作時間
        state = LocalState.WaitingForReel;
    }

    /*──────────────── Reel ▸ 收竿 ───────────────*/
    private IEnumerator ReelRoutine()
    {
        state = LocalState.Reeling;

        // 1) 模擬收竿時間 0.3s
        yield return new WaitForSeconds(0.3f);

        // 2) 摧毀魚標 + 關閉連線
        if (currentBobber) Destroy(currentBobber);
        line.enabled = false;

        // 3) 回到 Idle，可再次甩竿
        state = LocalState.Idle;
    }
}

/*───────────────────────────────────────────────────────────
 * FishingLine.cs – 簡易釣線 (連接兩端 Transform)
 * 需同物件加 LineRenderer
 *─────────────────────────────────────────────────────────*/
[RequireComponent(typeof(LineRenderer))]
public class FishingLine : MonoBehaviour
{
    private LineRenderer lr;
    private Transform a, b;
    private void Awake() { lr = GetComponent<LineRenderer>(); lr.positionCount = 2; }
    public void SetTargets(Transform tA, Transform tB) { a = tA; b = tB; }
    private void LateUpdate()
    {
        if (!a || !b) return;
        lr.SetPosition(0, a.position);
        lr.SetPosition(1, b.position);
    }
}
