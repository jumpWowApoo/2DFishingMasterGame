using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Inventory
{
    [DefaultExecutionOrder(-100)]
    public class InventoryMgr : MonoBehaviour
    {
        public static InventoryMgr Instance { get; private set; }

        [Header("背包格數量")] [SerializeField] int slotCount = 21;
        InventoryItemBase[] slots;

        /// <summary>格子變更事件，參數為格子索引</summary>
        public event Action<int> OnSlotChanged;

        void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            slots = new InventoryItemBase[slotCount];
        }

        public IReadOnlyList<InventoryItemBase> Items => slots;

        /// <summary>放入指定格，覆蓋原物品</summary>
        public void AddAt(int index, InventoryItemBase item)
        {
            if (index < 0 || index >= slots.Length) return;
            slots[index] = item;
            OnSlotChanged?.Invoke(index);
        }

        /// <summary>清空指定格</summary>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= slots.Length) return;
            slots[index] = null;
            OnSlotChanged?.Invoke(index);
        }

        /// <summary>交換兩個格子的物品</summary>
        public void Move(int from, int to)
        {
            if (from < 0 || from >= slots.Length || to < 0 || to >= slots.Length) return;
            var temp = slots[from];
            slots[from] = slots[to];
            slots[to] = temp;
            OnSlotChanged?.Invoke(from);
            OnSlotChanged?.Invoke(to);
        }

        /// <summary>清空全部背包</summary>
        public void Clear()
        {
            for (int i = 0; i < slots.Length; i++) {slots[i] = null; OnSlotChanged?.Invoke(i);}  
        }
        
        public bool RemoveFirst(string itemId)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] is FishItem fi && fi.id== itemId)
                {
                    slots[i] = null;
                    OnSlotChanged?.Invoke(i);
                    return true;
                }
            }
            Debug.LogWarning($"背包裡找不到 {itemId}");
            return false;
        }

        /// <summary>尋找第一個空格 index，找不到回傳 -1</summary>
        public int FirstEmptySlot()
        {
            for (int i = 0; i < slots.Length; i++)
                if (slots[i] == null) return i;
            return -1;
        }
    }
    
    
}