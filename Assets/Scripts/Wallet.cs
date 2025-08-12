using System;
using UnityEngine;

namespace Game.Currency
{
    public class Wallet : MonoBehaviour
    {
        public static Wallet Instance { get; private set; }

        [SerializeField] int gold = 0;
        public int Gold => gold;

        public event Action<int> OnGoldChanged;

        void Awake()
        {
            if (Instance && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            // 若要跨場景存活：DontDestroyOnLoad(gameObject);
            OnGoldChanged?.Invoke(gold);
        }

        public void Add(int amount)
        {
            if (amount <= 0) return;
            gold += amount;
            OnGoldChanged?.Invoke(gold);
        }

        public bool TrySpend(int amount)
        {
            if (amount <= 0) return true;
            if (gold < amount) return false;
            gold -= amount;
            OnGoldChanged?.Invoke(gold);
            return true;
        }

        public void ResetTo(int value = 0)
        {
            gold = Mathf.Max(0, value);
            OnGoldChanged?.Invoke(gold);
        }
    }
}