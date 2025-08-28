using System.Collections.Generic;
using UnityEngine;

namespace Game.Consumables.Shop
{
    [System.Serializable]
    public class CatalogItem
    {
        public ConsumableData data;
    }

    [CreateAssetMenu(menuName="Shop/Shop Database")]
    public class ShopDatabase : ScriptableObject
    {
        public List<CatalogItem> items = new();
        public int Count => items?.Count ?? 0;
        public CatalogItem Get(int i) => (i >= 0 && i < Count) ? items[i] : null;
    }
}