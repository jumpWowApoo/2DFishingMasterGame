using UnityEngine;
using UnityEngine.Serialization;

namespace Game.UI
{
    /// <summary>
    /// 結算/商店 面板中樞（結算商店場景專用）。
    /// - 進場：關閉其他面板 → 依設定開啟結算或商店
    /// - 對外 API：ShowSettlement / ShowShop / ToggleShop / CloseAll
    /// - keepSettlementAlwaysOpen：若沒有主要面板開啟且結算是關的，自動再打開結算
    /// </summary>
    public class UIHub_SettlementShop : MonoBehaviour
    {
        [Header("主要面板")]
        [SerializeField] GameObject settlementPanel;        // 結算面板（有 SettlementUI）
        [SerializeField] GameObject shopPanel;              // 商店面板（有 ShopUI）

        [FormerlySerializedAs("BagPlane")]
        [SerializeField] GameObject bagPanel;               // 背包面板（可選：顯示於商店時）

        [Header("其他面板（進場一律關閉，可留空）")]
        [SerializeField] GameObject[] otherPanels;

        [Header("行為選項")]
        [Tooltip("進場自動開啟結算面板（false=進場改開商店面板；若商店為空仍會回到結算）")]
        [SerializeField] bool openSettlementOnStart = true;

        [Tooltip("顯示商店時是否一併顯示背包面板")]
        [SerializeField] bool showBagWithShop = true;

        [Tooltip("若沒有其它主要面板開啟、且結算是關閉，會自動打開結算")]
        [SerializeField] bool keepSettlementAlwaysOpen = true;

        void OnEnable()
        {
            // 進場先關所有面板
            CloseAll();

            // 關附加指定的其它面板
            if (otherPanels != null)
                foreach (var p in otherPanels) SetActiveSafe(p, false);

            // 依設定開哪個主面板
            if (openSettlementOnStart)
            {
                ShowSettlement();
            }
            else
            {
                if (shopPanel) ShowShop();        // 若有商店 → 開商店
            }
        }

        void LateUpdate()
        {
            
        }

        // ===== 對外 API（拿去綁按鈕） =====
        public void ShowSettlement()
        {
            SetActiveSafe(shopPanel, false);
            SetActiveSafe(bagPanel, false);
            SetActiveSafe(settlementPanel, true);
        }

        public void ShowShop()
        {
            SetActiveSafe(shopPanel, true);
            SetActiveSafe(bagPanel, showBagWithShop && bagPanel);
        }

        public void ToggleShop()
        {
            shopPanel.SetActive(!shopPanel.activeSelf);
            bagPanel.SetActive(true);
            
        }

        public void CloseAll()
        {
            SetActiveSafe(settlementPanel, false);
            SetActiveSafe(shopPanel, false);
            SetActiveSafe(bagPanel, false);
        }

        // ===== 小工具 =====
        static void SetActiveSafe(GameObject go, bool on)
        {
            if (!go) return;
            if (go.activeSelf != on) go.SetActive(on);
        }
    }
}
