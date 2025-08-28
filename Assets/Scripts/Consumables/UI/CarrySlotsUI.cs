using UnityEngine;

namespace Game.Consumables.UI
{
    /// <summary>
    /// A 場景：7 格工具欄的 UI 控制。
    /// 修正：不在 Awake 建格；改在 OnEnable/Start 建，確保資料源已 Awake。
    /// </summary>
    [DisallowMultipleComponent]
    public class CarrySlotsUI : MonoBehaviour
    {
        [Header("資料來源")]
        [SerializeField] CarrySlots source;

        [Header("UI")]
        [SerializeField] Transform   grid;       // 容器（每格是 grid 的子物件）
        [SerializeField] CarrySlotUI slotPrefab; // 可選：不填則沿用現有子物件

        CarrySlotUI[] slots;
        bool built;

        void Awake()
        {
            if (!source) source = FindObjectOfType<CarrySlots>(true);
        }

        void OnEnable()
        {
            // 確保先建好再訂閱
            BuildOnce();
            if (source) source.Changed += RefreshAll;
            RefreshAll();
        }

        void Start()
        {
            // 避免部分情況 OnEnable 早於來源初始化，這裡再保險一次
            BuildOnce();
            RefreshAll();
        }

        void OnDisable()
        {
            if (source) source.Changed -= RefreshAll;
        }

        void BuildOnce()
        {
            if (built || !grid || !source) return;

            if (slotPrefab)
            {
                // 先清空
                for (int i = grid.childCount - 1; i >= 0; i--)
                    Destroy(grid.GetChild(i).gameObject);

                int n = source.Count;
                slots = new CarrySlotUI[n];
                for (int i = 0; i < n; i++)
                {
                    var s = Instantiate(slotPrefab, grid);
                    s.Bind(source, i);
                    slots[i] = s;
                }
            }
            else
            {
                int n = Mathf.Min(grid.childCount, source.Count);
                if (grid.childCount < source.Count)
                    Debug.LogWarning($"[CarrySlotsUI] Grid 子物件數({grid.childCount}) < CarrySlots.Count({source.Count})，只會建立前 {n} 格。若要 7 格請補齊子物件或指定 slotPrefab。", this);

                slots = new CarrySlotUI[n];
                for (int i = 0; i < n; i++)
                {
                    var t = grid.GetChild(i);
                    var s = t.GetComponent<CarrySlotUI>();
                    if (!s) s = t.gameObject.AddComponent<CarrySlotUI>();
                    s.Bind(source, i);
                    slots[i] = s;
                }
            }

            built = true;
        }

        public void RefreshAll()
        {
            if (slots == null || source == null) return;
            int n = Mathf.Min(slots.Length, source.Count);
            for (int i = 0; i < n; i++)
                slots[i].SetIcon(source.Get(i));
        }
    }
}
