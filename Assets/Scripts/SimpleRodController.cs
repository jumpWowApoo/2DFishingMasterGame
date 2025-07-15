using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(FishingLine))]
public class SimpleRodController : MonoBehaviour
{
    /*──────── Inspector 參數 ────────*/
    [Header("UI")]
    [SerializeField] private Button castButton;

    [SerializeField] private GameObject _buttobj;

    [Header("Transforms")]
    [SerializeField] private Transform rodTip;    // 釣竿尖端
    [SerializeField] private Transform targetPos; // 落點（空物件）

    [Header("Prefabs")]
    [SerializeField] private GameObject bobberPrefab; // 無 Rigidbody 的魚標 Prefab

    [Header("Motion Settings")]
    [Tooltip("飛行時間 (秒)")]
    [SerializeField] private float travelTime = 1.2f;
    [Tooltip("拋物線最高高度 (相對 rodTip)")]
    [SerializeField] private float arcHeight = 1.5f;

    /*──────── 私有變數 ─────────────*/
    private enum LocalState { Idle, Casting, WaitingForReel, Reeling }
    private LocalState state = LocalState.Idle;

    private GameObject  currentBobber;
    private FishingLine line;

    private void Awake()
    {
        castButton.onClick.AddListener(OnCastClicked);
        line = GetComponent<FishingLine>();
        line.Show(false);
    }

    /*──────── Update ────────────────*/
    private void Update()
    {
        if (state == LocalState.WaitingForReel && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(ReelRoutine());
        }
    }

    /*──────────────── Cast ─────────────────────────────*/
    private void OnCastClicked()
    {
        if (state != LocalState.Idle) return;
        if (!bobberPrefab || !rodTip || !targetPos) return;
        StartCoroutine(CastRoutine());
        _buttobj.SetActive(false);
        
    }

    private IEnumerator CastRoutine()
    {
        state = LocalState.Casting;

        // 1) 生成魚標
        currentBobber = Instantiate(bobberPrefab, rodTip.position, Quaternion.identity);

        // 2) 釣線連接並啟用
        line.SetTargets(rodTip, currentBobber.transform);
        line.Show(true);

        // 3) 拋物線移動
        yield return StartCoroutine(MoveBobber(currentBobber.transform));

        // 4) 到達落點，進入等待收竿
        Debug.Log("釣魚中");
        state = LocalState.WaitingForReel;
    }

    /*──────────────── MoveBobber – 程式化弧線 ─────────────*/
    private IEnumerator MoveBobber(Transform bob)
    {
        Vector3 start = rodTip.position;
        Vector3 end   = targetPos.position;

        for (float t = 0; t < 1f; t += Time.deltaTime / travelTime)
        {
            // 先做水平線性插值
            Vector3 pos = Vector3.Lerp(start, end, t);
            // 再加拋物線垂直位移 4h * (t - t^2)
            float yOffset = 4f * arcHeight * t * (1f - t);
            pos.y += yOffset;
            bob.position = pos;
            yield return null;
        }
        // 確保精準到達終點
        bob.position = end;
    }

    /*──────────────── Reel (收竿) ────────────────────────*/
    private IEnumerator ReelRoutine()
    {
        state = LocalState.Reeling;
        yield return new WaitForSeconds(0.3f); // 模擬收竿時間

        if (currentBobber) Destroy(currentBobber);
        line.Show(false);
        _buttobj.SetActive(true);
        state = LocalState.Idle;
    }
}