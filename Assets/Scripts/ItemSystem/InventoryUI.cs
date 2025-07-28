using Game.Inventory;
using UnityEngine;

namespace Game.UI
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] ItemSlot[] slots;

        void Awake()
        {
            slots = GetComponentsInChildren<ItemSlot>(true);
        }

        void OnEnable()
        {
            InventoryMgr.Instance.OnSlotChanged += OnSlotChanged;
            Refresh();
        }

        void OnDisable()
        {
            InventoryMgr.Instance.OnSlotChanged -= OnSlotChanged;
        }

        void OnSlotChanged(int index)
        {
            // 只刷新該格
            var item = InventoryMgr.Instance.Items[index];
            slots[index].Bind(index, item);
        }

        void Refresh()
        {
            var items = InventoryMgr.Instance.Items;
            for (int i = 0; i < slots.Length; i++)
            {
                var item = i < items.Count ? items[i] : null;
                slots[i].Bind(i, item);
            }
        }
    }
}