using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Consumables.UI
{
    public class IngameEquipSlotUI : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] Image icon;

        int index;

        public void Bind(int index) { this.index = index; }

        public void SetIcon(ConsumableData data)
        {
            if (icon)
            {
                icon.sprite  = data ? data.icon : null;
                icon.enabled = data != null;
            }
        }

        public void OnPointerClick(PointerEventData e)
        {
            if (e.button != PointerEventData.InputButton.Right) return;
            // 使用：直接呼叫 Use 並清除 Loadout 中該格
            var loadout = PlayerLoadout.Instance;
            if (loadout == null) return;

            var d = loadout.Get(index);
            if (d == null) return;

            d.Use(new ConsumableContext());
            loadout.Set(index, null);
            SetIcon(null);
        }
    }
}