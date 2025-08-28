using UnityEngine;

namespace Game.Consumables
{
    /// <summary>
    /// 將 CarrySlots（7 格）同步到 PlayerLoadout（1:1）。
    /// 建議：liveSyncOnCarryChanged = true、moveFromCarry = false（鏡像，不清空來源）
    /// 非單例版本：請在 Inspector 指定 carry 與 loadout；找不到會嘗試 FindObjectOfType(true) 一次。
    /// </summary>
    [DisallowMultipleComponent]
    public class CarryToLoadoutSync : MonoBehaviour
    {
        [Header("來源：7格工具槽")]
        [SerializeField] CarrySlots carry;

        [Header("目標：裝備欄（場景內的 PlayerLoadout）")]
        [SerializeField] PlayerLoadout loadout;

        [Header("行為")]
        [Tooltip("true = 搬移(同步後清空來源格)；false = 鏡像(保留來源)")]
        [SerializeField] bool moveFromCarry = false;

        [Tooltip("true = 只有目標該格為空才寫入；false = 直接覆蓋")]
        [SerializeField] bool onlyIfLoadoutEmpty = false;

        [Header("同步時機")]
        [SerializeField] bool syncOnEnable = true;
        [SerializeField] bool liveSyncOnCarryChanged = true;

        [Header("偵錯")]
        [SerializeField] bool logVerbose = false;

        bool _guard;
        bool _subscribed;

        void Reset() => TryResolveRefs();

        void OnValidate()
        {
            // 編輯器狀態下若有遺失，嘗試補一次（不影響執行期效能）
            if (!Application.isPlaying) TryResolveRefs();
        }

        void OnEnable()
        {
            TryResolveRefs();

            if (liveSyncOnCarryChanged && carry && !_subscribed)
            {
                carry.Changed += RunSyncNow;
                _subscribed = true;
            }

            if (syncOnEnable) RunSyncNow();
        }

        void OnDisable()
        {
            if (_subscribed && carry)
                carry.Changed -= RunSyncNow;
            _subscribed = false;
        }

        void TryResolveRefs()
        {
            if (!carry)   carry   = FindObjectOfType<CarrySlots>(true);
            if (!loadout) loadout = FindObjectOfType<PlayerLoadout>(true);
        }

        /// <summary>手動觸發完整同步（Inspector 右鍵可點）。</summary>
        [ContextMenu("Run Sync Now")]
        public void RunSyncNow()
        {
            if (_guard) return;

            if (!carry || !loadout)
            {
                if (logVerbose)
                    Debug.LogWarning($"[CarryToLoadoutSync] 缺少參考 carry={carry} loadout={loadout}", this);
                return;
            }

            _guard = true;

            int n = Mathf.Min(carry.Count, loadout.Count);
            for (int i = 0; i < n; i++)
                SyncSlot(i);

            _guard = false;
        }

        /// <summary>只同步單一格（拖曳時可呼叫）。</summary>
        public void SyncSlot(int index)
        {
            if (!carry || !loadout) return;
            if (index < 0 || index >= carry.Count || index >= loadout.Count) return;

            var src = carry.Get(index);
            var dst = loadout.Get(index);

            if (src == null)
            {
                // 來源為空 → 依需求保持目標不動
                if (logVerbose) Debug.Log($"[CarryToLoadoutSync] slot {index}: 來源空，略過（目標保持 {(dst ? dst.name : "空")}）", this);
                return;
            }

            if (onlyIfLoadoutEmpty && dst != null)
            {
                if (logVerbose) Debug.Log($"[CarryToLoadoutSync] slot {index}: 目標非空({dst.name})，依設定跳過", this);
                return;
            }

            loadout.Set(index, src);
            if (logVerbose) Debug.Log($"[CarryToLoadoutSync] slot {index}: 寫入 {src.name}", this);

            if (moveFromCarry)
            {
                // 注意：這會觸發 carry.Changed → 但有 _guard 保護不會重入
                carry.TakeAt(index);
                if (logVerbose) Debug.Log($"[CarryToLoadoutSync] slot {index}: 已自來源搬移（清空來源）", this);
            }
        }
    }
}
