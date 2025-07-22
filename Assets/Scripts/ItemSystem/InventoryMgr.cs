using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Inventory
{ 
    /// <summary>背包資料層 (Singleton)。</summary>
    [DefaultExecutionOrder(-100)]

    public class InventoryMgr : MonoBehaviour
    {
        public static InventoryMgr Instance { get; private set; }

        readonly List<InventoryItemBase> items = new();
        public IReadOnlyList<InventoryItemBase> Items => items;

        public event Action<InventoryItemBase> OnAdd;
        public event Action<int> OnRemove;

        void Awake()
        {
            if (Instance && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Add(InventoryItemBase item)
        {
            items.Add(item);
            OnAdd?.Invoke(item);
        }

        public void RemoveAt(int idx)
        {
            if (idx < 0 || idx >= items.Count) return;
            items.RemoveAt(idx);
            OnRemove?.Invoke(idx);
        }

        /// <summary>若你想「一次只能有一條魚」，可直接清空再 Add。</summary>
        public void Clear() => items.Clear();
    }
}