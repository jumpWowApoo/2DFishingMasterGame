using UnityEngine;

namespace Game.UI
{
    /// <summary>
    /// 結算/商店 面板中樞（結算商店場景專用）。
    /// - 場景啟用時：關閉其它面板 → 開啟結算面板
    /// - 提供公開方法：ShowSettlement / ShowShop / ToggleShop / CloseAll
    /// - 選配：keepSettlementAlwaysOpen=true 時，若結算面板被關掉且沒有其它面板在顯示，會自動再打開
    /// </summary>
    public class UIHub_SettlementShop : MonoBehaviour
    {
        [Header("主要面板")]
        [SerializeField] GameObject settlementPanel;   // 結算面板（有 SettlementUI）
        [SerializeField] GameObject shopPanel;         // 商店面板（有 ShopUI）

        [Header("其他需要在進場時一律關閉的面板（可留空）")]
        [SerializeField] GameObject[] otherPanels;

        [Header("行為選項")]
        [Tooltip("進場時自動開啟結算面板、並關閉其它面板")]
        [SerializeField] bool openSettlementOnStart = true;

        [Tooltip("若結算面板被關閉，且目前沒有其它主要面板開啟，就自動再打開結算面板")]
        [SerializeField] bool keepSettlementAlwaysOpen = true;

        void OnEnable()
        {
            settlementPanel.SetActive(true);
            shopPanel.SetActive(false);
        }

        void LateUpdate()
        {
            if (!keepSettlementAlwaysOpen) return;

            bool anyMainOpen = (shopPanel && shopPanel.activeSelf);
            if (!anyMainOpen && settlementPanel && !settlementPanel.activeSelf)
            {
                // 沒有其它主要面板在顯示，又發現結算被關了 → 重新打開結算
                settlementPanel.SetActive(true);
            }
        }

        // ===== 對外 API（拿去綁按鈕） =====
        public void ShowSettlement()
        {
            SetActiveSafe(shopPanel, false);
        }

        public void ShowShop()
        {
            SetActiveSafe(settlementPanel, false);
            SetActiveSafe(shopPanel, true);
        }

        public void ToggleShop()
        {
            if (!shopPanel) return;
            bool target = !shopPanel.activeSelf;
            if (target) ShowShop();
            else ShowSettlement();
        }

        public void CloseAll()
        {
            SetActiveSafe(settlementPanel, false);
            SetActiveSafe(shopPanel, false);
        }

        // ===== 小工具 =====

        static void SetActiveSafe(GameObject go, bool on)
        {
            if (go && go.activeSelf != on) go.SetActive(on);
        }
    }
}
