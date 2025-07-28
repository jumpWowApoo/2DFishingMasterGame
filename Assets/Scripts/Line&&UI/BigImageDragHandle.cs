using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class BigImageDragHandle : MonoBehaviour,
        IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("拖影生成父層 (DragLayer)")]
        [SerializeField] RectTransform dragRoot;      // 指向 Canvas/DragLayer

        FishItem   item;         // 目前拖的魚
        GameObject dragVisual;   // 拖影實例

        static FishItem currentDragged;
        public  static FishItem CurrentDragged => currentDragged;

        public void Init(FishItem item, RectTransform root = null)
        {
            this.item = item;
            if (root) dragRoot = root;
        }

        public void OnBeginDrag(PointerEventData e)
        {
            if (item == null || dragRoot == null) return;

            dragVisual = Instantiate(item.data.dragItemPrefab, dragRoot);

            // 換成該魚的小圖
            if (dragVisual.TryGetComponent(out Image img))
                img.sprite = item.Icon;

            var cg = dragVisual.GetComponent<CanvasGroup>() ??
                     dragVisual.AddComponent<CanvasGroup>();
            cg.blocksRaycasts = false;

            dragVisual.transform.position = e.position;
            currentDragged = item;
        }

        public void OnDrag(PointerEventData e)
        {
            if (dragVisual) dragVisual.transform.position = e.position;
        }

        public void OnEndDrag(PointerEventData e)
        {
            if (dragVisual) Destroy(dragVisual);
            dragVisual    = null;
            currentDragged = null;
        }
    }
}