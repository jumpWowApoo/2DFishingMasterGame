using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Consumables.UI
{
    /// <summary>
    /// B 場景：裝備欄單一格。
    /// 右鍵：使用並清空格子。
    /// 拖曳：支援 Loadout↔Loadout、Bag→Loadout、Carry→Loadout。
    /// </summary>
    public class IngameEquipSlotUI : MonoBehaviour,
        IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
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
            if (!icon) icon = GetComponentInChildren<Image>(true);
            if (!icon) return;
            icon.sprite  = data ? data.icon : null;
            icon.enabled = data != null;
        }

        // ===== 右鍵使用（B 場景）=====
        public void OnPointerClick(PointerEventData e)
        {
            if (e.button != PointerEventData.InputButton.Right) return;
            if (!loadout) return;

            var d = loadout.Get(index);
            if (d == null) return;

            d.Use(new ConsumableContext());
            loadout.Set(index, null); // 觸發 IngameEquipBar 透過 loadout.Changed 全列刷新
            SetIcon(null);            // 當幀即時刷新，避免殘影
        }

        // ===== 拖曳開始 =====
        public void OnBeginDrag(PointerEventData e)
        {
            if (!loadout) return;

            var data = loadout.Get(index);
            if (data == null || data.icon == null) return;

            var dh = DragHandle.Instance; if (!dh) return;

            dh.BeginDrag(data.icon, new DragHandle.Payload
            {
                kind    = DragHandle.SourceKind.Loadout,
                loadout = loadout,
                index   = index,
                data    = data
            });
            dh.Move(e.position);
        }

        // ===== 拖曳移動 =====
        public void OnDrag(PointerEventData e)
        {
            var dh = DragHandle.Instance; if (dh) dh.Move(e.position);
        }

        // ===== 拖曳結束（沒投遞到可用目標時收起）=====
        public void OnEndDrag(PointerEventData e)
        {
            var dh = DragHandle.Instance; if (dh) dh.EndDrag();
        }

        // ===== 接受投遞 =====
        public void OnDrop(PointerEventData e)
        {
            var dh = DragHandle.Instance;
            if (!dh || dh.Current == null || loadout == null) return;

            var cur = dh.Current;

            // 1) Loadout → Loadout
            if (cur.kind == DragHandle.SourceKind.Loadout && cur.loadout != null)
            {
                // 同一顆 Loadout：交換
                if (cur.loadout == loadout)
                {
                    if (cur.index == index) return;
                    var a = loadout.Get(cur.index);
                    var b = loadout.Get(index);
                    loadout.Set(index, a);
                    loadout.Set(cur.index, b);
                    return;
                }
                // 不同 Loadout（少見，也支援）
                else
                {
                    var a = cur.loadout.Get(cur.index);
                    var b = loadout.Get(index);
                    loadout.Set(index, a);
                    cur.loadout.Set(cur.index, b);
                    return;
                }
            }

            // 2) Bag → Loadout
            if (cur.kind == DragHandle.SourceKind.Bag && cur.bag != null)
            {
                var src = cur.bag.TakeAt(cur.index); // 來源清空
                if (src == null) return;

                var dst = loadout.Get(index);
                loadout.Set(index, src);

                // 若本格原本有東西，把它放回來源的原位
                if (dst != null) cur.bag.PutAt(cur.index, dst);
                return;
            }

            // 3) Carry → Loadout
            if (cur.kind == DragHandle.SourceKind.Carry && cur.carry != null)
            {
                var src = cur.carry.TakeAt(cur.index); // 來源清空
                if (src == null) return;

                var dst = loadout.Get(index);
                loadout.Set(index, src);

                if (dst != null) cur.carry.PutAt(cur.index, dst);
                return;
            }
        }
    }
}
