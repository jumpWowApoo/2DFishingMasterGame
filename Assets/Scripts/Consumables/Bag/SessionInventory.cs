using Game.Consumables;

namespace Game.Session
{
    /// <summary>
    /// 跨場景暫存（記憶體內，不需要單例物件）。
    /// </summary>
    public static class SessionInventory
    {
        public static ConsumableData[] CarrySlots   = new ConsumableData[7];   // A 的 7 格（要帶去 B）
        public static ConsumableData[] LoadoutSlots = new ConsumableData[7];   // B 的 7 格（用完後剩餘，回 A 回填）
        public static ConsumableData[] BagSlots     = new ConsumableData[28];  // A 的背包（跨場景共用）

        public static bool HasAny(ConsumableData[] arr)
        {
            if (arr == null) return false;
            for (int i = 0; i < arr.Length; i++) if (arr[i] != null) return true;
            return false;
        }

        public static int CountNonNull(ConsumableData[] arr)
        {
            if (arr == null) return 0;
            int c = 0; for (int i = 0; i < arr.Length; i++) if (arr[i] != null) c++;
            return c;
        }
    }
}