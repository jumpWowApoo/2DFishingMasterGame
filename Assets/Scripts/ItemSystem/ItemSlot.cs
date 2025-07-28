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
        [SerializeField] Image icon;          // 小圖
        [SerializeField] Text  label;         // 名稱 (可選)

        int index;

        void Awake()
        {
            if (icon == null) icon = GetComponent<Image>();
        }

        /* 刷新格子 */
        public void Bind(int idx, InventoryItemBase item)
        {
            index = idx;

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

        /* 右鍵移除 */
        public void OnPointerClick(PointerEventData e)
        {
            if (e.button == PointerEventData.InputButton.Right)
                InventoryMgr.Instance.RemoveAt(index);
        }

        /* 接收從大圖或其他格拖來的魚 */
        public void OnDrop(PointerEventData e)
        {
            var dragged = BigImageDragHandle.CurrentDragged;
            if (dragged == null) return;

            InventoryMgr.Instance.Add(dragged);
            Bind(index, dragged);                 // 立即顯示
        }
    }
}