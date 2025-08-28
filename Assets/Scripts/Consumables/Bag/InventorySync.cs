using UnityEngine;
using Game.Consumables;

namespace Game.Session
{
    /// <summary>
    /// A / B 與 Session 的讀寫工具。
    /// </summary>
    public static class InventorySync
    {
        // ===== A → Session =====
        public static void SaveAtoSession(CarrySlots carry, ConsumableBag bag)
        {
            if (carry != null)
            {
                EnsureSize(ref SessionInventory.CarrySlots, carry.Count);
                for (int i = 0; i < carry.Count; i++)
                    SessionInventory.CarrySlots[i] = carry.Get(i);
            }

            if (bag != null)
            {
                EnsureSize(ref SessionInventory.BagSlots, bag.Capacity);
                for (int i = 0; i < bag.Capacity; i++)
                    SessionInventory.BagSlots[i] = bag.GetAt(i);
            }
        }

        // ===== Session → B =====
        public static void LoadSessionToB(PlayerLoadout loadout)
        {
            if (loadout == null || SessionInventory.CarrySlots == null) return;
            int n = Mathf.Min(loadout.Count, SessionInventory.CarrySlots.Length);
            for (int i = 0; i < n; i++)
                loadout.Set(i, SessionInventory.CarrySlots[i]);
        }

        // ===== B → Session =====
        public static void SaveBtoSession(PlayerLoadout loadout)
        {
            if (loadout == null) return;
            EnsureSize(ref SessionInventory.LoadoutSlots, loadout.Count);
            for (int i = 0; i < loadout.Count; i++)
                SessionInventory.LoadoutSlots[i] = loadout.Get(i);
        }

        // ===== 回到 A：把 B 剩餘裝備回填到 7 格；占用就塞背包 =====
        // 注意：這不會動到 Session.BagSlots；請在外面自己 SaveAtoSession。
        public static void ReturnSessionLoadoutToA(CarrySlots carry, ConsumableBag bag, bool verbose = true)
        {
            var src = SessionInventory.LoadoutSlots;
            if (src == null) return;

            for (int i = 0; i < src.Length; i++)
            {
                var d = src[i];
                if (!d) continue;

                bool placed = false;

                // 優先回原 index 的 A 7格，只有空位才放
                if (carry != null && i < carry.Count && carry.Get(i) == null)
                {
                    carry.Set(i, d);
                    placed = true;
                    if (verbose) Debug.Log($"[ReturnLoadout] slot {i}: 回填到 A 的 7格");
                }

                // 若該位占用，嘗試塞進 A 背包
                if (!placed && bag != null)
                {
                    if (bag.TryAdd(d))
                    {
                        placed = true;
                        if (verbose) Debug.Log($"[ReturnLoadout] slot {i}: 該位占用 → 放入 A 背包");
                    }
                    else if (verbose)
                    {
                        Debug.LogWarning($"[ReturnLoadout] slot {i}: 背包已滿，{d.name} 無處可放");
                    }
                }

                // 處理完清掉來源（無論是否成功）
                src[i] = null;
            }
        }

        // ===== Session 背包 → A 背包（覆蓋）=====
        public static void LoadSessionBagToA(ConsumableBag bag)
        {
            if (bag == null || SessionInventory.BagSlots == null) return;

            bag.ClearAll(); // 需要你的 ConsumableBag 有 ClearAll()
            for (int i = 0; i < SessionInventory.BagSlots.Length; i++)
            {
                var d = SessionInventory.BagSlots[i];
                if (d) bag.TryAdd(d);
            }
        }

        static void EnsureSize(ref ConsumableData[] arr, int size)
        {
            if (arr == null || arr.Length != size)
                arr = new ConsumableData[size];
        }
    }
}
