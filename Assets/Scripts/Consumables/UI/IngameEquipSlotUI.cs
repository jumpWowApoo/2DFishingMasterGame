using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Consumables.UI
{
    /// <summary>
    /// 裝備欄單一格（遊戲場景 B）。右鍵：使用並清空格子。
    /// </summary>
    public class IngameEquipSlotUI : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] Image icon;

        PlayerLoadout loadout;
        int index;

        public void Bind(PlayerLoadout loadout, int index)
        {
            this.loadout = loadout;
            this.index   = index;
            SetIcon(loadout ? loadout.Get(index) : null);
        }

        public void SetIcon(ConsumableData data)
        {
            if (!icon) icon = GetComponentInChildren<Image>();
            if (!icon) return;
            icon.sprite  = data ? data.icon : null;
            icon.enabled = data != null;
        }

        public void OnPointerClick(PointerEventData e)
        {
            if (e.button != PointerEventData.InputButton.Right) return;
            if (!loadout) return;

            var d = loadout.Get(index);
            if (d == null) return;

            d.Use(new ConsumableContext());
            loadout.Set(index, null); // 會觸發 IngameEquipBar 全列刷新
            SetIcon(null);            // 當幀即時刷新，避免殘影
        }
    }
}