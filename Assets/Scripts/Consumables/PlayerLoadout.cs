using UnityEngine;

namespace Game.Consumables
{
    public class PlayerLoadout : MonoBehaviour
    {
        public static PlayerLoadout Instance { get; private set; }

        [SerializeField, Min(1)] int slotCount = 7;
        [SerializeField] ConsumableData[] slots;

        public int Count => slotCount;

        void Awake()
        {
            if (Instance && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (slots == null || slots.Length != slotCount)
                slots = new ConsumableData[slotCount];
        }

        public ConsumableData Get(int idx) =>
            (idx >= 0 && idx < slots.Length) ? slots[idx] : null;

        public void Set(int idx, ConsumableData data)
        {
            if (idx < 0 || idx >= slots.Length) return;
            slots[idx] = data;
        }

        public void ClearAll()
        {
            for (int i = 0; i < slots.Length; i++) slots[i] = null;
        }
    }
}