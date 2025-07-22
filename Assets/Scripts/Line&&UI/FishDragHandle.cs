using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class FishDragHandle : MonoBehaviour,
        IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] RectTransform dragRoot; // 建議指向 Canvas 下空物件

        FishItem     item;
        GameObject   dragVisual;
        CanvasGroup  cg;

        public void Init(FishItem item) => this.item = item;

        public void OnBeginDrag(PointerEventData e)
        {
            dragVisual = Instantiate(item.data.dragItemPrefab, dragRoot);
            cg = dragVisual.GetComponent<CanvasGroup>() ?? dragVisual.AddComponent<CanvasGroup>();
            cg.blocksRaycasts = false;
            dragVisual.transform.position = e.position;
        }

        public void OnDrag(PointerEventData e)
        {
            if (dragVisual) dragVisual.transform.position = e.position;
        }

        public void OnEndDrag(PointerEventData e)
        {
            if (dragVisual) Destroy(dragVisual);
        }
    }
}