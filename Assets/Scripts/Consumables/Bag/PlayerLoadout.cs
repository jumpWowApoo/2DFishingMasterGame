using System;
using UnityEngine;

namespace Game.Consumables
{
    /// <summary>
    /// B 場景的 7 格裝備欄（非單例）。
    /// </summary>
    public class PlayerLoadout : MonoBehaviour
    {
        [SerializeField, Min(1)] int slotCount = 7;
        [SerializeField] ConsumableData[] slots;

        public int Count => slotCount;

        public event Action Changed;
        void RaiseChanged() => Changed?.Invoke();

        void Awake()
        {
            if (slots == null || slots.Length != slotCount)
                slots = new ConsumableData[slotCount];
        }

        public ConsumableData Get(int idx) =>
            (idx >= 0 && idx < slots.Length) ? slots[idx] : null;

        public void Set(int idx, ConsumableData data)
        {
            if (idx < 0 || idx >= slots.Length) return;
            slots[idx] = data;
            RaiseChanged();
        }

        public void ClearAll()
        {
            for (int i = 0; i < slots.Length; i++) slots[i] = null;
            RaiseChanged();
        }
    }
}