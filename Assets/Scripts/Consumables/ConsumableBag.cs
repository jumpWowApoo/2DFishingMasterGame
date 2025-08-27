using UnityEngine;

namespace Game.Consumables
{
    public class ConsumableBag : MonoBehaviour
    {
        [SerializeField, Min(1)] int capacity = 28;
        [SerializeField] ConsumableData[] slots;

        public int Capacity => capacity;

        void Awake()
        {
            if (slots == null || slots.Length != capacity)
                slots = new ConsumableData[capacity];
        }

        public bool TryAdd(ConsumableData data)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] == null) { slots[i] = data; return true; }
            }
            return false;
        }

        public bool UseAt(int index, ConsumableContext ctx = null)
        {
            if (index < 0 || index >= slots.Length) return false;
            var d = slots[index];
            if (d == null) return false;

            d.Use(ctx ?? new ConsumableContext());
            slots[index] = null;
            return true;
        }

        public ConsumableData GetAt(int index) =>
            (index >= 0 && index < slots.Length) ? slots[index] : null;

        public void ClearAll()
        {
            for (int i = 0; i < slots.Length; i++) slots[i] = null;
        }
    }
}