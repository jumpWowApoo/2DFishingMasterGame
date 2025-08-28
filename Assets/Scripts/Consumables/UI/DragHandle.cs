using UnityEngine;
using UnityEngine.UI;

namespace Game.Consumables.UI
{
    /// <summary>
    /// 通用拖曳控制（拖影 + 載荷）：支援 Bag / Carry / Loadout 三種來源。
    /// </summary>
    public class DragHandle : MonoBehaviour
    {
        public static DragHandle Instance { get; private set; }

        [Header("拖影父層（建議 Canvas/UIHub/DragLayer）")]
        [SerializeField] RectTransform dragLayer;

        Image proxy;
        Canvas rootCanvas;

        public enum SourceKind { Bag, Carry, Loadout }

        public class Payload
        {
            public SourceKind kind;
            public Game.Consumables.ConsumableBag bag;
            public Game.Consumables.CarrySlots carry;
            public Game.Consumables.PlayerLoadout loadout;
            public int index;
            public ConsumableData data;
        }

        public Payload Current { get; private set; }

        void Awake()
        {
            if (Instance && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            rootCanvas = GetComponentInParent<Canvas>();
        }

        public void BeginDrag(Sprite icon, Payload p)
        {
            EndDrag();
            Current = p;

            RectTransform parent = dragLayer ? dragLayer : (rootCanvas ? rootCanvas.transform as RectTransform : null);
            if (!parent) return;

            var go = new GameObject("DragProxy", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(CanvasGroup));
            go.transform.SetParent(parent, false);
            proxy = go.GetComponent<Image>();
            proxy.raycastTarget = false;
            proxy.sprite = icon;
            proxy.SetNativeSize();
            go.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }

        public void Move(Vector2 screenPos)
        {
            if (!proxy) return;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                proxy.canvas.transform as RectTransform,
                screenPos, proxy.canvas.worldCamera, out var local);
            proxy.rectTransform.anchoredPosition = local;
        }

        public void EndDrag()
        {
            if (proxy) Destroy(proxy.gameObject);
            proxy = null;
            Current = null;
        }
    }
}
