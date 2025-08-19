using UnityEngine;
using UnityEngine.UI;
using Game.Inventory;
using Game.Currency;
using UnityEngine.SceneManagement;

public class SettlementUI : MonoBehaviour
{
    [Header("Root")] [SerializeField] GameObject panel;

    [Header("List")] [SerializeField] Transform listRoot; // ScrollView Content
    [SerializeField] SettlementRow rowPrefab; // 行 Prefab（掛 SettlementRow）

    [Header("Total")] [SerializeField] Text txtTotal;

    [Header("Buttons")] [SerializeField] Button btnConfirm;
    [SerializeField] Button btnClose; // 可選


    void Awake()
    {
        if (btnConfirm) btnConfirm.onClick.AddListener(OnClickConfirm);
        if (btnClose) btnClose.onClick.AddListener(CloseOnly);
    }

    void OnEnable()
    {
        Rebuild();
    }

    public void Open()
    {
        if (panel) panel.SetActive(true);
        Rebuild();
    }

    void Rebuild()
    {
        // 清掉舊列
        for (int i = listRoot.childCount - 1; i >= 0; i--)
            Destroy(listRoot.GetChild(i).gameObject);

        int totalFish = 0;

        // ===== 1) 魚清單（在上方）=====
        if (FishCrate.I != null)
        {
            var snap = FishCrate.I.GetSnapshot();
            foreach (var kv in snap)
            {
                var d = kv.Key;
                int c = kv.Value;
                int sum = d.sellPrice * c;
                totalFish += sum;

                var row = Instantiate(rowPrefab, listRoot);
                row.Bind(d.fishName, d.sellPrice, c, sum);
            }
        }
        else
        {
            Debug.LogWarning("[SettlementUI] FishCrate.I is null — 檢查是否在第一個場景且有 DontDestroyOnLoad。");
        }

        // ===== 2) 任務清單（永遠放在最後）=====
        int missionTotal = 0;
        if (SessionRunLog.I != null)
        {
            var list = SessionRunLog.I.GetAggregatedSnapshot();
            var counts = SessionRunLog.I.GetCounts();      // (kinds, times)
            missionTotal = SessionRunLog.I.ComputeMissionTotal();

            // 如果你想在任務區塊加一條分隔標題（可選）：
            // var header = Instantiate(rowPrefab, listRoot);
            // header.Bind($"— 任務完成（{counts.kinds} 種 | {counts.times} 次）—", 0, 0, 0);

            foreach (var m in list)
            {
                int perReward = (m.times > 0) ? (m.rewardSum / m.times) : 0;  // 每次獎勵（目前多半為 0）
                // 用 SettlementRow 四欄對應：標題 / 每次獎勵 / 次數 / 累計獎勵
                var row = Instantiate(rowPrefab, listRoot);
                row.Bind(m.title, perReward, m.times, m.rewardSum);
            }
        }
        else
        {
            // 沒有 SessionRunLog 也不影響魚顯示，只是不會有任務列
            Debug.LogWarning("[SettlementUI] SessionRunLog.I is null — 若需顯示任務完成，請加入 SessionRunLog 單例。");
        }

        // ===== 3) 總金額（魚 + 任務獎勵；目前任務獎勵多為 0）=====
        int total = totalFish + missionTotal;
        if (txtTotal) txtTotal.text = $"總金額 {total}";

        if (btnConfirm) btnConfirm.interactable = true;
    }



    void OnClickConfirm()
    {
        int total = FishCrate.I.ComputeTotalPrice();

        // ★ 用你的錢包
        Wallet.Instance.Add(total);

        // 只結一次 → 清空魚箱
        FishCrate.I.Clear();

        InventoryMgr.Instance.Clear();

        // 關閉結算場景，回到遊戲
        //SettlementFlow.ReturnToGame();
        SceneManager.LoadScene("S1");
        
    }

    void CloseOnly()
    {
        // 不結算直接返回（可移除）
        SettlementFlow.ReturnToGame();
    }
}