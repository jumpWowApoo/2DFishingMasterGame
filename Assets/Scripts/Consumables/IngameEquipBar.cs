using UnityEngine;

namespace Game.Consumables.UI
{
    public class IngameEquipBar : MonoBehaviour
    {
        [SerializeField] Transform slotHolder;
        [SerializeField] IngameEquipSlotUI slotPrefab;

        IngameEquipSlotUI[] slots;

        void Start()
        {
            Build();
            PullFromPlayer();
        }

        void Build()
        {
            foreach (Transform c in slotHolder) Destroy(c.gameObject);

            int n = PlayerLoadout.Instance ? PlayerLoadout.Instance.Count : 0;
            slots = new IngameEquipSlotUI[n];
            for (int i = 0; i < n; i++)
            {
                var s = Instantiate(slotPrefab, slotHolder);
                s.Bind(i);
                slots[i] = s;
            }
        }

        public void PullFromPlayer()
        {
            if (slots == null || PlayerLoadout.Instance == null) return;
            for (int i = 0; i < slots.Length; i++)
                slots[i].SetIcon(PlayerLoadout.Instance.Get(i));
        }
    }
}