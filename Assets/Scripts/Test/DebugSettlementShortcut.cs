using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class DebugSettlementShortcut : MonoBehaviour
{
    [Tooltip("僅在 Editor 或 Development Build 啟用。打勾可在 Release 也啟用（不建議）。")]
    public bool enableInRelease = false;

    [Tooltip("按下 Ctrl+Shift+F1 時，先放入一些測試魚再進結算。")]
    public bool addSampleFishOnShift = true;

    void Update()
    {
        // 僅在 Editor/Development Build 生效，除非強制開啟
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        bool allowed = true;
#else
        bool allowed = enableInRelease;
#endif
        if (!allowed) return;

        bool ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        if (ctrl && Input.GetKeyDown(KeyCode.F1))
        {
            // 可選：Ctrl+Shift+F1 先灌幾條魚，避免空結算
            if (addSampleFishOnShift && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                TryAddSampleFishForDemo();
            }

            Debug.Log("[Debug] Ctrl+F1 → Open Settlement (normal flow)");
            SettlementFlow.OpenSettlement(); // ★ 走你的正常結算流程
        }
    }

    void TryAddSampleFishForDemo()
    {
        if (FishCrate.I == null) return;

        // 如果你的專案有集中管理 FishData 的方式，改成從你的 DB 取。
        // 這裡只示範：找場上任意 FishData 來塞幾條，避免結算列表是空的。
        var anyFish = Resources.FindObjectsOfTypeAll<FishData>();
        if (anyFish != null && anyFish.Length > 0)
        {
            // 加兩種魚各 3 條當範例
            FishCrate.I.Add(anyFish[0], 3);
            if (anyFish.Length > 1) FishCrate.I.Add(anyFish[1], 3);
            Debug.Log("[Debug] Injected sample fish into FishCrate for demo.");
        }
        else
        {
            Debug.Log("[Debug] No FishData found to inject sample fish.");
        }
    }
}