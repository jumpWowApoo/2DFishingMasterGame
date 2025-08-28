using UnityEngine;

namespace Game.Consumables.UI
{
    /// <summary>
    /// 背包 UI 控制：建格子一次、訂閱 bag.Changed 即時刷新。
    /// 使用現有的 ConsumableSlotUI。
    /// </summary>
    [DisallowMultipleComponent]
    public class ConsumableBagUI : MonoBehaviour
    {
        [Header("資料來源")]
        [SerializeField] ConsumableBag bag;

        [Header("UI")]
        [SerializeField] Transform grid;                 // GridLayoutGroup 容器
        [SerializeField] ConsumableSlotUI slotPrefab;    // 可留空：沿用現有子物件

        ConsumableSlotUI[] slots;
        bool built;

        void Awake()
        {
            if (!bag) bag = FindObjectOfType<ConsumableBag>(true);
        }

        void OnEnable()
        {
            BuildOnce();
            if (bag) bag.Changed += RefreshAll;
            RefreshAll();
        }

        void Start()
        {
            // 部分情況 OnEnable 早於來源初始化，保險再建一次
            BuildOnce();
            RefreshAll();
        }

        void OnDisable()
        {
            if (bag) bag.Changed -= RefreshAll;
        }

        void BuildOnce()
        {
            if (built || !grid || !bag) return;

            if (slotPrefab)
            {
                // 清空舊有子物件
                for (int i = grid.childCount - 1; i >= 0; i--)
                    Destroy(grid.GetChild(i).gameObject);

                slots = new ConsumableSlotUI[bag.Capacity];
                for (int i = 0; i < bag.Capacity; i++)
                {
                    var s = Instantiate(slotPrefab, grid);
                    // A 場景背包：右鍵不使用
                    s.SetAllowRightClickUse(false);
                    s.Bind(bag, i);
                    slots[i] = s;
                }
            }
            else
            {
                int n = Mathf.Min(grid.childCount, bag.Capacity);
                if (grid.childCount < bag.Capacity)
                    Debug.LogWarning($"[ConsumableBagUI] Grid 子物件數({grid.childCount}) < Bag.Capacity({bag.Capacity})，僅顯示前 {n} 格。建議指定 slotPrefab 生成。", this);

                slots = new ConsumableSlotUI[n];
                for (int i = 0; i < n; i++)
                {
                    var t = grid.GetChild(i);
                    var s = t.GetComponent<ConsumableSlotUI>();
                    if (!s) s = t.gameObject.AddComponent<ConsumableSlotUI>();
                    s.SetAllowRightClickUse(false); // A 場景背包：右鍵不使用
                    s.Bind(bag, i);
                    slots[i] = s;
                }
            }

            built = true;
        }

        public void RefreshAll()
        {
            if (slots == null || bag == null) return;
            int n = Mathf.Min(slots.Length, bag.Capacity);
            for (int i = 0; i < n; i++)
                slots[i].Refresh();
        }
    }
}
