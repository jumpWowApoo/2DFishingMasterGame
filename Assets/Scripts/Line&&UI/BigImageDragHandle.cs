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

        public void Init(FishItem item, RectTransform root = null)
        {
            this.item = item;
            dragRoot = root ?? UIHub.Instance?.DragLayer;
        }

        public void OnBeginDrag(PointerEventData e)
        {
            if (item == null || dragRoot == null) return;

            dragVisual = Instantiate(item.data.dragItemPrefab, dragRoot);
            if (dragVisual.TryGetComponent(out Image img))
                img.sprite = item.Icon;

            var cg = dragVisual.GetComponent<CanvasGroup>() ?? dragVisual.AddComponent<CanvasGroup>();
            cg.blocksRaycasts = false;
            dragVisual.transform.position = e.position;

            DragInfo.CurrentDragged = item;
            DragInfo.OriginSlotIndex = -1;
            DragInfo.FromInventory = false;
        }

        public void OnDrag(PointerEventData e)
        {
            if (dragVisual) dragVisual.transform.position = e.position;
        }

        public void OnEndDrag(PointerEventData e)
        {
            if (dragVisual) Destroy(dragVisual);
            dragVisual = null;
            Debug.Log("[BigImageDragHandle] OnEndDrag");
            DragInfo.CurrentDragged = null;
        }
    }
}