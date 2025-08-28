using System;
using UnityEngine;

namespace Game.Consumables
{
    /// <summary>
    /// A 場景用：要帶去 B 的 7 格工具欄（非單例）。
    /// 修正：所有存取都先 EnsureArray()；執行順序提前，避免 UI 先於此 Awake。
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class CarrySlots : MonoBehaviour
    {
        [SerializeField, Min(1)] int slotCount = 7;
        [SerializeField] ConsumableData[] slots;

        public int Count => slotCount;

        public event Action Changed;
        void RaiseChanged() => Changed?.Invoke();

        void Awake()
        {
            EnsureArray();
        }

        void EnsureArray()
        {
            if (slotCount < 1) slotCount = 1;
            if (slots == null || slots.Length != slotCount)
                slots = new ConsumableData[slotCount];
        }

        public ConsumableData Get(int i)
        {
            EnsureArray();
            if (i < 0 || i >= slots.Length) return null;
            return slots[i];
        }

        public void Set(int i, ConsumableData d)
        {
            EnsureArray();
            if (i < 0 || i >= slots.Length) return;
            slots[i] = d;
            RaiseChanged();
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

        public bool PutAt(int i, ConsumableData d)
        {
            EnsureArray();
            if (i < 0 || i >= slots.Length) return false;
            if (slots[i] != null) return false;
            slots[i] = d;
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
            // 在編輯器裡改 slotCount 時，讓陣列長度跟上
            if (!Application.isPlaying) EnsureArray();
        }
#endif
    }
}
