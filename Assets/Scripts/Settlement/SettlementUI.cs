using UnityEngine;
using UnityEngine.UI;
using Game.Common;
using Game.Currency;

// using UnityEngine.SceneManagement; // 不需要直接載場景，交給 SettlementFlow

public class SettlementUI : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] GameObject panel;

    [Header("List")]
    [SerializeField] Transform listRoot;       // ScrollView Content
    [SerializeField] SettlementRow rowPrefab;  // 行 Prefab（掛 SettlementRow）

    [Header("Total")]
    [SerializeField] Text txtTotal;

    [Header("Buttons")]
    [SerializeField] Button btnConfirm; // 結算完成：入錢包 + 清魚箱 + 關閉面板（留在結算場景）
    [SerializeField] Button btnClose;   // 回到遊戲：切回上一個遊戲場景（重載＝初始化）

    [Header("準備流程 UI")]
    [Tooltip("背包面板（如：Panel_Bag，內有 ConsumableBagUI）")]
    [SerializeField] GameObject bagPanel;
    [Tooltip("道具欄面板（如：Panel_Carry7 或 Panel_Loadout）")]
    [SerializeField] GameObject loadoutPanel;
    [Tooltip("『出門』按鈕的 GameObject；若不指定，會自動使用 btnClose.gameObject")]
    [SerializeField] GameObject exitButtonGO;
    [Tooltip("『準備』按鈕；按下後會顯示 背包/道具欄/出門按鈕")]
    [SerializeField] Button btnPrepare;
    [Tooltip("啟用時是否先隱藏 背包/道具欄/出門按鈕，等按『準備』再顯示")]
    [SerializeField] bool hidePrepUIOnEnable = true;

    void Awake()
    {
        if (btnConfirm) btnConfirm.onClick.AddListener(OnClickConfirmAndClosePanel);
        if (btnClose)   btnClose  .onClick.AddListener(OnClickReturnToGame);
        if (btnPrepare) btnPrepare.onClick.AddListener(OnClickPrepare);

        // 若沒有手動指定出門按鈕物件，預設使用 btnClose
        if (!exitButtonGO && btnClose) exitButtonGO = btnClose.gameObject;
    }

    void OnEnable()
    {
        Rebuild();

        if (hidePrepUIOnEnable)
            SetPrepUIVisible(false); // 先藏起來，按「準備」再顯示
    }

    void Rebuild()
    {
        if (!listRoot) return;

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

                if (rowPrefab)
                {
                    var row = Instantiate(rowPrefab, listRoot);
                    row.Bind(d.fishName, d.sellPrice, c, sum);
                }
            }
        }
        else
        {
            Debug.LogWarning("[SettlementUI] FishCrate.I is null — 檢查是否第一個場景且有 DontDestroyOnLoad。");
        }

        // ===== 2) 任務清單（永遠放在最後）=====
        int missionTotal = 0;
        if (SessionRunLog.I != null)
        {
            var list = SessionRunLog.I.GetAggregatedSnapshot();
            // var counts = SessionRunLog.I.GetCounts(); // (kinds, times) // 目前未顯示，可保留需求時使用
            missionTotal = SessionRunLog.I.ComputeMissionTotal();

            if (rowPrefab)
            {
                foreach (var m in list)
                {
                    int perReward = (m.times > 0) ? (m.rewardSum / m.times) : 0;
                    var row = Instantiate(rowPrefab, listRoot);
                    row.Bind(m.title, perReward, m.times, m.rewardSum);
                }
            }
        }
        else
        {
            Debug.LogWarning("[SettlementUI] SessionRunLog.I is null — 若需顯示任務完成，請加入 SessionRunLog 單例。");
        }

        // ===== 3) 總金額（魚 + 任務獎勵）=====
        int total = totalFish + missionTotal;
        if (txtTotal) txtTotal.text = $"總金額 {total}";

        if (btnConfirm) btnConfirm.interactable = true;
    }

    /// <summary>
    /// 按『準備』：顯示 背包 / 道具欄 / 出門按鈕。可依需求把『準備』本身關掉避免重複按。
    /// </summary>
    void OnClickPrepare()
    {
        SetPrepUIVisible(true);
        if (btnPrepare) btnPrepare.gameObject.SetActive(false);
    }

    void SetPrepUIVisible(bool on)
    {
        if (bagPanel)     bagPanel.SetActive(on);
        if (loadoutPanel) loadoutPanel.SetActive(on);
        if (exitButtonGO) exitButtonGO.SetActive(on);
    }

    /// <summary>
    /// 結算完成：把錢入錢包、清魚箱，然後「關閉結算面板」但仍留在結算場景。
    /// </summary>
    void OnClickConfirmAndClosePanel()
    {
        if (FishCrate.I != null)
        {
            int total = FishCrate.I.ComputeTotalPrice();
            Wallet.Instance.Add(total);
            FishCrate.I.Clear(); // 下一輪才不會重算
        }

        if (btnConfirm) btnConfirm.interactable = false; // 防重按
        if (panel) panel.SetActive(false);               // 關閉結算畫面（仍在結算場景）

        Debug.Log("[SettlementUI] 結算完成，面板已關閉。");
    }

    /// <summary>
    /// 回到遊戲：切回上一個遊戲場景（Single）→ 自動初始化該場景內所有物件。
    /// </summary>
    void OnClickReturnToGame()
    {
        // 可選清理單局統計（若你有）
        // if (SessionRunLog.I != null) SessionRunLog.I.Clear();
        SceneReturnContext.Reset = ResetLevel.Soft; // 告訴 LevelInitializer：這次要重置
        SettlementFlow.ReturnToGame();              // Single 載回上一個場景
    }
}
