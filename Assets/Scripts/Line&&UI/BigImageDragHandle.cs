using Game.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class BigImageDragHandle : MonoBehaviour,
        IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] RectTransform dragRoot;
        FishItem item;
        GameObject dragVisual;
        Image dragImg;

        public void Init(FishItem item, RectTransform root = null)
        {
            this.item = item;
            dragRoot = root ?? UIHub.Instance?.DragLayer;
        }

        public void OnBeginDrag(PointerEventData e)
        {
            if (item == null || item.data == null || dragRoot == null) return;

            // 建立拖影
            dragVisual = Instantiate(item.data.dragItemPrefab, dragRoot);
            dragImg = dragVisual.GetComponent<Image>() ?? dragVisual.AddComponent<Image>();
            dragImg.sprite = item.Icon;          // 初始顯示魚的正常圖示
            dragImg.raycastTarget = false;       // 讓拖影不擋住投放事件

            var cg = dragVisual.GetComponent<CanvasGroup>() ?? dragVisual.AddComponent<CanvasGroup>();
            cg.blocksRaycasts = false;

            dragVisual.transform.position = e.position;

            // 設定拖曳上下文，給各 Drop/覆蓋層使用
            DragInfo.CurrentDragged   = item;
            DragInfo.OriginSlotIndex  = -1;      // 大圖拖曳，不是從背包來
            DragInfo.FromInventory    = false;
            DragInfo.CurrentDragImage = dragImg; // ★ 提供給 DiscardOverlay 切換丟棄圖示
        }

        public void OnDrag(PointerEventData e)
        {
            if (dragVisual) dragVisual.transform.position = e.position;
        }

        public void OnEndDrag(PointerEventData e)
        {
            if (dragVisual) Destroy(dragVisual);
            dragVisual = null;

            // 清理拖曳上下文
            DragInfo.CurrentDragImage = null;
            DragInfo.CurrentDragged   = null;

            if (UIHub.Instance != null) UIHub.Instance.CloseFishInfo();
        }
    }
}