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

        private FishItem   item;
        private int        originSlot;
        private GameObject dragVisual;
        private Image      dragImg;    // ★ 拖影圖，給 DiscardOverlay 改成丟棄圖示
        private CanvasGroup cg;

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

            // 產生拖影
            dragVisual = Instantiate(item.data.dragItemPrefab, dragRoot);
            dragImg    = dragVisual.GetComponent<Image>() ?? dragVisual.AddComponent<Image>();
            dragImg.sprite        = item.Icon;
            dragImg.raycastTarget = false; // ★ 拖影不擋 Drop

            cg = dragVisual.GetComponent<CanvasGroup>() ?? dragVisual.AddComponent<CanvasGroup>();
            cg.blocksRaycasts = false;

            dragVisual.transform.position = e.position;

            // 設定拖曳資訊（給目標槽 / DiscardOverlay 使用）
            DragInfo.CurrentDragged   = item;
            DragInfo.OriginSlotIndex  = originSlot;
            DragInfo.FromInventory    = true;          // ★ 來源是背包
            DragInfo.CurrentDragImage = dragImg;       // ★ 讓 DiscardOverlay 能替換成丟棄圖示
        }

        public void OnDrag(PointerEventData e)
        {
            if (dragVisual) dragVisual.transform.position = e.position;
        }

        public void OnEndDrag(PointerEventData e)
        {
            if (dragVisual) Destroy(dragVisual);

            // 清除拖曳狀態（若丟到 DiscardOverlay，它會先處理扣除）
            DragInfo.CurrentDragImage = null;
            DragInfo.CurrentDragged   = null;
            DragInfo.FromInventory    = false;
            DragInfo.OriginSlotIndex  = -1;
        }
    }
}
