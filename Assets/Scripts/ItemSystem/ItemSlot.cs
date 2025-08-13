using Game.Inventory;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
    [RequireComponent(typeof(Image))]
    public class ItemSlot : MonoBehaviour,
        IPointerClickHandler, IDropHandler
    {
        [Header("UI Refs")]
        [SerializeField] Image icon;
        [SerializeField] Text  label;

        int index;

        void Awake()
        {
            if (icon == null) icon = GetComponent<Image>();
        }

        public void Bind(int idx, InventoryItemBase item)
        {
            index = idx;

            // 先初始化拖曳把手 (只有魚可拖)
            if (item is FishItem fish)
            {
                var drag = icon.GetComponent<FishDragHandle>();
                if (drag != null)
                    drag.Init(fish, idx);
            }

            // 再顯示小圖／文字
            if (item != null)
            {
                icon.enabled = true;
                icon.sprite  = item.Icon;
                if (label)
                {
                    label.enabled = true;
                    label.text    = item.Name;
                }
            }
            else
            {
                icon.enabled = false;
                icon.sprite  = null;
                if (label) label.enabled = false;
            }
        }

        public void OnPointerClick(PointerEventData e)
        {
            if (e.button == PointerEventData.InputButton.Right)
            {
                // ★ 同步魚箱：若刪的是魚，魚箱也要扣 1，避免結算還算到錢
                var item = InventoryMgr.Instance.Items[index];
                if (item is FishItem fish && FishCrate.I != null)
                {
                    FishCrate.I.Remove(fish.data, 1);
                }

                InventoryMgr.Instance.RemoveAt(index);
            }
        }

        public void OnDrop(PointerEventData e)
        {
            var fish = DragInfo.CurrentDragged;
            if (fish == null) return;

            if (DragInfo.FromInventory)
            {
                // 背包內移動，不影響魚箱
                InventoryMgr.Instance.Move(DragInfo.OriginSlotIndex, index);
            }
            else
            {
                // 從外部（例如任務格）拖回來 → 只是回到背包，魚箱不變
                InventoryMgr.Instance.AddAt(index, fish);
            }

            Bind(index, fish);
            DragInfo.CurrentDragged = null;
        }
    }
}
