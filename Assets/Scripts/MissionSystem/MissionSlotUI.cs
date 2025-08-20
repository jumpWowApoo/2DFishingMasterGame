using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Game.Inventory;

[RequireComponent(typeof(Image))] // 任務格底圖
public class MissionSlotUI : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    [Header("UI")] [SerializeField] Image icon;
    public FishItem HeldItem { get; private set; }
    public bool HasItem => HeldItem != null; 

    public event Action OnItemChanged; // 通知 MissionUI

    void Awake()
    {
        if (icon == null) icon = GetComponent<Image>();
        icon.enabled = false; // 預設隱藏
    }

    /* ───── 拖入判斷 ───── */
    public void OnDrop(PointerEventData ev)
    {
        FishItem fish = DragInfo.CurrentDragged;
        if (fish == null) return;
        HeldItem = fish;  
        
        /* 來源在背包 → 清空原格 */
        if (DragInfo.FromInventory)
            InventoryMgr.Instance.RemoveAt(DragInfo.OriginSlotIndex);

        /* 受理此魚 → 顯示 icon */
        HeldItem = fish;
        icon.sprite = fish.Icon;
        icon.enabled = true;

        OnItemChanged?.Invoke();
        DragInfo.CurrentDragged = null;
        AudioHub.I.PlayUi(UiSfx.PutInCrate);
    }

    /* ───── 右鍵 → 退回背包 ───── */
    public void OnPointerClick(PointerEventData ev)
    {
        if (ev.button != PointerEventData.InputButton.Right) return;
        if (HeldItem == null) return;

        int empty = InventoryMgr.Instance.FirstEmptySlot();
        if (empty >= 0)
        {
            InventoryMgr.Instance.AddAt(empty, HeldItem);
            HeldItem = null;

            /* 隱藏小圖 */
            icon.enabled = false;
            icon.sprite = null;

            OnItemChanged?.Invoke();
        }
        // 若背包已滿，可在此做提示
    }
    
    public void ResetSlot(string newAcceptId)
    {
        HeldItem     = null;          // 清空邏輯
        icon.enabled = false;         // 隱藏小圖
        icon.sprite  = null;
    }
}