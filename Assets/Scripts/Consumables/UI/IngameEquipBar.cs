using UnityEngine;

namespace Game.Consumables.UI
{
    /// <summary>
    /// 遊戲場景中的 7 格裝備欄 UI。
    /// </summary>
    [DisallowMultipleComponent]
    public class IngameEquipBar : MonoBehaviour
    {
        [Header("資料來源")]
        [SerializeField] PlayerLoadout loadout;   // 指向同場景那顆 PlayerLoadout（非單例）

        [Header("UI")]
        [SerializeField] Transform slotHolder;
        [SerializeField] IngameEquipSlotUI slotPrefab; // 可選：不設就沿用子物件

        IngameEquipSlotUI[] slots;
        bool built;

        void Awake()
        {
            if (!loadout) loadout = GetComponentInParent<PlayerLoadout>();
            if (!loadout) loadout = FindObjectOfType<PlayerLoadout>(true);
            BuildOnce();
        }

        void OnEnable()
        {
            if (loadout) loadout.Changed += PullFromPlayer;
            PullFromPlayer(); // 只刷新圖示，不重建、不讀 Session
        }

        void OnDisable()
        {
            if (loadout) loadout.Changed -= PullFromPlayer;
        }

        void BuildOnce()
        {
            if (built || !slotHolder || !loadout) return;

            if (slotPrefab)
            {
                for (int i = slotHolder.childCount - 1; i >= 0; i--)
                    Destroy(slotHolder.GetChild(i).gameObject);

                int n = loadout.Count;
                slots = new IngameEquipSlotUI[n];
                for (int i = 0; i < n; i++)
                {
                    var s = Instantiate(slotPrefab, slotHolder);
                    s.Bind(loadout, i);
                    slots[i] = s;
                }
            }
            else
            {
                int count = slotHolder.childCount;
                slots = new IngameEquipSlotUI[count];
                for (int i = 0; i < count; i++)
                {
                    var child = slotHolder.GetChild(i);
                    var s = child.GetComponent<IngameEquipSlotUI>();
                    if (!s) s = child.gameObject.AddComponent<IngameEquipSlotUI>();
                    s.Bind(loadout, i);
                    slots[i] = s;
                }
            }

            built = true;
        }

        public void PullFromPlayer()
        {
            if (slots == null || loadout == null) return;
            int n = Mathf.Min(slots.Length, loadout.Count);
            for (int i = 0; i < n; i++)
                slots[i].SetIcon(loadout.Get(i));
        }
    }
}
