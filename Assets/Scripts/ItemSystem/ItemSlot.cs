using Game.Inventory;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
    [RequireComponent(typeof(Image))]
    public class ItemSlot : MonoBehaviour, IPointerClickHandler
    {
        [Header("UI Refs")]
        [SerializeField] Image icon;         // 小圖示
        [SerializeField] Text  label;     
        int index;

        void Awake()    // 自動抓缺少的元件
        {
            if (icon == null)  icon  = GetComponent<Image>();
        }

        /// <summary>綁定物品（或清空）。</summary>
        public void Bind(int idx, InventoryItemBase item)
        {
            index = idx;

            if (item != null)
            {
                icon.enabled = true;
                icon.sprite  = item.Icon;

                if (label)    // 顯示名稱
                {
                    label.enabled = true;
                    label.text    = item.Name;
                }
            }
            else            // 空格：隱藏圖與字
            {
                icon.enabled  = false;
                icon.sprite   = null;

                if (label)    label.enabled = false;
            }
        }

        public void OnPointerClick(PointerEventData e)
        {
            if (e.button == PointerEventData.InputButton.Right)
                InventoryMgr.Instance.RemoveAt(index);
        }
    }
}