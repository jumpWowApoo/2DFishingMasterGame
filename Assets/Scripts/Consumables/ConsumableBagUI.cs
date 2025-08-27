using UnityEngine;

namespace Game.Consumables.UI
{
    public class ConsumableBagUI : MonoBehaviour
    {
        [SerializeField] ConsumableBag bag;
        [SerializeField] Transform slotHolder;
        [SerializeField] ConsumableSlotUI slotPrefab;

        ConsumableSlotUI[] slots;

        void Start()
        {
            if (!bag) bag = FindObjectOfType<ConsumableBag>(true);
            Build();
            RefreshAll();
        }

        void Build()
        {
            foreach (Transform c in slotHolder) Destroy(c.gameObject);
            int n = bag != null ? bag.Capacity : 0;
            slots = new ConsumableSlotUI[n];
            for (int i = 0; i < n; i++)
            {
                var s = Instantiate(slotPrefab, slotHolder);
                s.Bind(bag, i);
                slots[i] = s;
            }
        }

        public void RefreshAll()
        {
            if (slots == null) return;
            foreach (var s in slots) s?.Refresh();
        }
    }
}