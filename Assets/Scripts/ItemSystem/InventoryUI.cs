using Game.Inventory;
using UnityEngine;

namespace Game.UI
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] ItemSlot[] slots;
        void Awake()
        {
            /* 在自己（InventoryWindow）底下抓所有啟用的 ItemSlot */
            slots = GetComponentsInChildren<ItemSlot>(includeInactive: true);
        }
        void OnEnable()
        {
            InventoryMgr.Instance.OnAdd    += _ => Refresh();
            InventoryMgr.Instance.OnRemove += _ => Refresh();
            Refresh();
        }
        void OnDisable()
        {
            InventoryMgr.Instance.OnAdd    -= _ => Refresh();
            InventoryMgr.Instance.OnRemove -= _ => Refresh();
        }

        void Refresh()
        {
            var list = InventoryMgr.Instance.Items;
            for (int i = 0; i < slots.Length; i++)
                slots[i].Bind(i, i < list.Count ? list[i] : null);
        }
    }
}