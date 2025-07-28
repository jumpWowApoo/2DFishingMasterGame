/// <summary>
/// 區分背包系統的魚&&視窗的魚
/// </summary>
public static class DragInfo
{
    public static FishItem CurrentDragged;     // 正在拖的 FishItem
    public static int      OriginSlotIndex;    // 背包來源格索引，-1=視窗拖
    public static bool     FromInventory;      // true=背包拖動，false=視窗拖動
}