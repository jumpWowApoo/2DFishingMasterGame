using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class FishDragHandle : MonoBehaviour,
        IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] RectTransform dragRoot;
        FishItem   item;
        int        originSlot;
        GameObject dragVisual;
        CanvasGroup cg;

        /// <summary>初始化要拖的物件及來源格</summary>
        public void Init(FishItem item, int slotIndex)
        {
            this.item       = item;
            this.originSlot = slotIndex;
            if (dragRoot == null && UIHub.Instance != null)
                dragRoot = UIHub.Instance.DragLayer;
        }

        public void OnBeginDrag(PointerEventData e)
        {
            if (item == null || dragRoot == null) return;

            dragVisual = Instantiate(item.data.dragItemPrefab, dragRoot);
            if (dragVisual.TryGetComponent(out Image img))
                img.sprite = item.Icon;
            cg = dragVisual.GetComponent<CanvasGroup>() ?? dragVisual.AddComponent<CanvasGroup>();
            cg.blocksRaycasts = false;
            dragVisual.transform.position = e.position;

            // 設定拖曳資訊
            DragInfo.CurrentDragged  = item;
            DragInfo.OriginSlotIndex = originSlot;
            DragInfo.FromInventory   = true;
        }

        public void OnDrag(PointerEventData e)
        {
            if (dragVisual) dragVisual.transform.position = e.position;
        }

        public void OnEndDrag(PointerEventData e)
        {
            Destroy(dragVisual);             // 清掉拖影
            DragInfo.CurrentDragged = null;  // 清除狀態
        }
    }
}