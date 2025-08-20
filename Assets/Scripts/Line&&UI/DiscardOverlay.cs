using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Game.Inventory;

/// <summary>
/// 常駐全螢幕丟棄層：
/// - 指標拖曳進入時：拖影圖示 → 換成「棄魚圖」
/// - 指標拖曳離開時：拖影圖示 → 還原
/// - 在此層放開（Drop）：刪除 1 條魚（背包來源清格，非背包來源扣 FishCrate）
/// 需求：此物件需有 Image（透明、Raycast Target 勾選），放在 UI 最上層以便接到 Drop。
/// </summary>
public class DiscardOverlay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    [Header("後備用：若該魚無自帶丟棄圖示時使用")]
    [SerializeField] private Sprite fallbackDiscardSprite;

    private Sprite prevSprite; // 拖影還原用

    // 進入覆蓋層：把拖影換成「棄魚圖」
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (DragInfo.CurrentDragged == null || DragInfo.CurrentDragImage == null) return;

        // 1) 優先取：目前被拖曳魚的 FishData.discardSprite
        // 2) 沒有則用本元件的 fallbackDiscardSprite
        var fishItem = DragInfo.CurrentDragged;
        Sprite discardSprite = (fishItem.data != null && fishItem.data.discardSprite != null)
            ? fishItem.data.discardSprite
            : fallbackDiscardSprite;

        if (discardSprite == null) return;

        prevSprite = DragInfo.CurrentDragImage.sprite;
        DragInfo.CurrentDragImage.sprite = discardSprite;
    }

    // 離開覆蓋層：還原拖影圖
    public void OnPointerExit(PointerEventData eventData)
    {
        if (DragInfo.CurrentDragImage != null && prevSprite != null)
        {
            DragInfo.CurrentDragImage.sprite = prevSprite;
            prevSprite = null;
        }
    }

    // 在覆蓋層放開：視為丟棄（-1）
    public void OnDrop(PointerEventData eventData)
    {
        if (DragInfo.CurrentDragged == null) return;

        var fishItem = DragInfo.CurrentDragged;

        bool removed = false;

        if (DragInfo.FromInventory && InventoryMgr.Instance != null && DragInfo.OriginSlotIndex >= 0)
        {
            // 來源：背包 → 先記下要扣的魚種
            var data = fishItem.data;

            // 1) 清背包格（視覺/持有面向）
            InventoryMgr.Instance.RemoveAt(DragInfo.OriginSlotIndex);

            // 2) 同步從魚箱扣 1（結算面向的真實數據）
            if (FishCrate.I != null)
                removed = FishCrate.I.Remove(data, 1) > 0;
            else
                removed = true; // 沒有魚箱也至少清了背包
        }
        else
        {
            // 來源：非背包（例如資訊窗）→ 只需要從魚箱扣 1
            if (FishCrate.I != null)
                removed = FishCrate.I.Remove(fishItem.data, 1) > 0;
        }

        // （可選）音效
        // AudioHub.I?.PlayUi(removed ? UiSfx.Discard : UiSfx.Error);

        // 拖影還原 + 清拖曳上下文
        if (DragInfo.CurrentDragImage != null && prevSprite != null)
            DragInfo.CurrentDragImage.sprite = prevSprite;

        prevSprite = null;
        DragInfo.CurrentDragImage = null;
        DragInfo.CurrentDragged   = null;
    }

}
