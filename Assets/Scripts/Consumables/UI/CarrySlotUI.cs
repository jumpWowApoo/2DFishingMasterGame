using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Consumables.UI
{
    /// <summary>
    /// A 場景：7 格工具欄單格。右鍵不做事；支援拖曳。
    /// - 來源 Carry→Carry：交換
    /// - 來源 Bag→Carry：放置（若本格有物件則與原位交換）
    /// </summary>
    public class CarrySlotUI : MonoBehaviour,
        IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        [SerializeField] Image icon;

        CarrySlots src;
        int index;

        public void Bind(CarrySlots source, int index)
        {
            this.src   = source;
            this.index = index;
            SetIcon(src ? src.Get(index) : null);
        }

        public void SetIcon(ConsumableData d)
        {
            if (!icon) icon = GetComponentInChildren<Image>(true);
            if (!icon) return;
            icon.sprite  = d ? d.icon : null;
            icon.enabled = d != null;
        }

        // A 場景右鍵不做事
        public void OnPointerClick(PointerEventData e) { /* no-op by design */ }

        // ===== 拖曳開始 =====
        public void OnBeginDrag(PointerEventData e)
        {
            if (src == null) return;

            var data = src.Get(index);
            if (data == null || data.icon == null) return;

            var dh = DragHandle.Instance; if (!dh) return;

            dh.BeginDrag(data.icon, new DragHandle.Payload
            {
                kind   = DragHandle.SourceKind.Carry,
                carry  = src,
                index  = index,
                data   = data
            });

            dh.Move(e.position);
        }

        // ===== 拖曳移動 =====
        public void OnDrag(PointerEventData e)
        {
            var dh = DragHandle.Instance; if (dh) dh.Move(e.position);
        }

        // ===== 拖曳結束（未送達時收起拖影）=====
        public void OnEndDrag(PointerEventData e)
        {
            var dh = DragHandle.Instance; if (dh) dh.EndDrag();
        }

        // ===== 投遞到本格 =====
        public void OnDrop(PointerEventData e)
        {
            var dh = DragHandle.Instance;
            if (!dh || dh.Current == null || src == null) return;

            var cur = dh.Current;

            // 1) Carry → Carry（同/不同來源都處理）
            if (cur.kind == DragHandle.SourceKind.Carry && cur.carry != null)
            {
                // 同一顆 CarrySlots
                if (cur.carry == src)
                {
                    if (cur.index == index) return; // 同格，不動
                    var a = src.TakeAt(cur.index);
                    var b = src.TakeAt(index);
                    if (a != null) src.PutAt(index, a);
                    if (b != null) src.PutAt(cur.index, b);
                    return;
                }
                // 不同 CarrySlots（少見，但一併支援）
                else
                {
                    var a = cur.carry.TakeAt(cur.index);
                    var b = src.TakeAt(index);
                    if (a != null) src.PutAt(index, a);
                    if (b != null) cur.carry.PutAt(cur.index, b);
                    return;
                }
            }

            // 2) Bag → Carry
            if (cur.kind == DragHandle.SourceKind.Bag && cur.bag != null)
            {
                var fromBag = cur.bag.TakeAt(cur.index); // 來源清空
                if (fromBag == null) return;

                var here = src.TakeAt(index);            // 取出本格原物
                src.PutAt(index, fromBag);               // 放到 Carry

                // 若本格原本有東西，把它放回來源的原位
                if (here != null) cur.bag.PutAt(cur.index, here);
                return;
            }

            // 3) Loadout → Carry（通常 A 場景沒有，但保守支援）
            if (cur.kind == DragHandle.SourceKind.Loadout && cur.loadout != null)
            {
                var fromLoadout = cur.loadout.Get(cur.index);
                if (fromLoadout == null) return;

                var here = src.TakeAt(index);
                src.PutAt(index, fromLoadout);
                cur.loadout.Set(cur.index, here); // 可以是 null（清空）
                return;
            }
        }
    }
}
