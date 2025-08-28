using System;
using UnityEngine;

namespace Game.Consumables
{
    /// <summary>
    /// A 場景 28 格背包（非單例）。僅存 ConsumableData，不含堆疊。
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class ConsumableBag : MonoBehaviour
    {
        [SerializeField, Min(1)] int capacity = 28;
        [SerializeField] ConsumableData[] slots;

        public int Capacity => capacity;

        public event Action Changed;
        void RaiseChanged() => Changed?.Invoke();

        void Awake() => EnsureArray();

        void EnsureArray()
        {
            if (capacity < 1) capacity = 1;
            if (slots == null || slots.Length != capacity)
                slots = new ConsumableData[capacity];
        }

        public ConsumableData GetAt(int i)
        {
            EnsureArray();
            if (i < 0 || i >= slots.Length) return null;
            return slots[i];
        }

        public bool PutAt(int i, ConsumableData d)
        {
            EnsureArray();
            if (i < 0 || i >= slots.Length) return false;
            if (slots[i] != null) return false;
            slots[i] = d;
            RaiseChanged();
            return true;
        }

        public ConsumableData TakeAt(int i)
        {
            EnsureArray();
            if (i < 0 || i >= slots.Length) return null;
            var d = slots[i];
            if (d != null)
            {
                slots[i] = null;
                RaiseChanged();
            }
            return d;
        }

        /// <summary>放進第一個空位；成功回 true。</summary>
        public bool TryAdd(ConsumableData d)
        {
            if (!d) return false;
            EnsureArray();
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] == null)
                {
                    slots[i] = d;
                    RaiseChanged();
                    return true;
                }
            }
            return false;
        }

        /// <summary>右鍵使用支援（若某些 UI 允許右鍵使用）。</summary>
        public bool UseAt(int i)
        {
            EnsureArray();
            if (i < 0 || i >= slots.Length) return false;
            var d = slots[i];
            if (d == null) return false;

            d.Use(new ConsumableContext());
            slots[i] = null; // 使用後移除
            RaiseChanged();
            return true;
        }

        public void ClearAll()
        {
            EnsureArray();
            for (int i = 0; i < slots.Length; i++) slots[i] = null;
            RaiseChanged();
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (!Application.isPlaying) EnsureArray();
        }
#endif
    }
}
