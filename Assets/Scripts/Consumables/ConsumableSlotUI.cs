using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Consumables.UI
{
    public class ConsumableSlotUI : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] Image icon;
        ConsumableBag bag;
        int index;

        public void Bind(ConsumableBag bag, int index)
        {
            this.bag = bag;
            this.index = index;
            Refresh();
        }

        public void Refresh()
        {
            var d = bag?.GetAt(index);
            if (icon)
            {
                icon.sprite  = d ? d.icon : null;
                icon.enabled = d != null;
            }
        }

        // 右鍵使用（遊戲場景可用；商店場景想禁用可把此腳本關閉）
        public void OnPointerClick(PointerEventData e)
        {
            if (e.button != PointerEventData.InputButton.Right) return;
            if (bag != null && bag.UseAt(index))
                Refresh();
        }
    }
}