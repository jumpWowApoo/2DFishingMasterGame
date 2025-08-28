using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Consumables.UI
{
    public class ConsumableSlotUI : MonoBehaviour,
        IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        [Header("Refs")]
        [SerializeField] Image icon;

        [Header("Behavior")]
        [SerializeField] bool allowRightClickUse = false; // A 場景背包=關閉

        ConsumableBag bag;
        int index;

        public void SetAllowRightClickUse(bool on) => allowRightClickUse = on;
        public void Bind(ConsumableBag bag, int index) { this.bag = bag; this.index = index; Refresh(); }

        public void Refresh()
        {
            var data = bag?.GetAt(index);
            if (!icon) return;
            var sp = (data != null) ? data.icon : null;
            icon.sprite  = sp;
            icon.enabled = (sp != null);
        }

        public void OnPointerClick(PointerEventData e)
        {
            if (!allowRightClickUse) return;
            if (e.button != PointerEventData.InputButton.Right) return;
            if (bag != null && bag.UseAt(index)) Refresh();
        }

        // ==== 拖曳 ====
        public void OnBeginDrag(PointerEventData e)
        {
            var data = bag?.GetAt(index);
            if (data == null || data.icon == null) return;

            var dh = DragHandle.Instance; if (!dh) return;
            dh.BeginDrag(data.icon, new DragHandle.Payload {
                kind = DragHandle.SourceKind.Bag,
                bag = bag,
                index = index,
                data = data
            });
            dh.Move(e.position);
        }

        public void OnDrag(PointerEventData e)
        {
            var dh = DragHandle.Instance; if (dh) dh.Move(e.position);
        }

        public void OnEndDrag(PointerEventData e)
        {
            var dh = DragHandle.Instance; if (dh) dh.EndDrag();
        }

        public void OnDrop(PointerEventData e)
        {
            var dh = DragHandle.Instance;
            if (!dh || dh.Current == null) return;

            var cur = dh.Current;

            // 來源：Bag → Bag
            if (cur.kind == DragHandle.SourceKind.Bag && cur.bag != null)
            {
                if (cur.bag == bag)
                {
                    if (cur.index == index) return;
                    var a = bag.TakeAt(cur.index);
                    var b = bag.TakeAt(index);
                    bag.PutAt(index, a);
                    if (b != null) bag.PutAt(cur.index, b);
                    return;
                }
                else
                {
                    var a = cur.bag.TakeAt(cur.index);
                    var b = bag.TakeAt(index);
                    bag.PutAt(index, a);
                    if (b != null) cur.bag.PutAt(cur.index, b);
                    return;
                }
            }
            // 來源：Carry → Bag
            else if (cur.kind == DragHandle.SourceKind.Carry && cur.carry != null)
            {
                var src = cur.carry.TakeAt(cur.index);
                var dst = bag.TakeAt(index);
                bag.PutAt(index, src);
                if (dst != null) cur.carry.PutAt(cur.index, dst);
                return;
            }
            // 來源：Loadout → Bag
            else if (cur.kind == DragHandle.SourceKind.Loadout && cur.loadout != null)
            {
                var src = cur.loadout.Get(cur.index);
                if (src == null) return;
                var dst = bag.TakeAt(index);
                bag.PutAt(index, src);
                cur.loadout.Set(cur.index, dst);
                return;
            }
        }

        public bool HasIcon() => icon != null;
#if UNITY_EDITOR
        public void EditorOnly_SetIcon(Image img) => icon = img;
#endif
    }
}
