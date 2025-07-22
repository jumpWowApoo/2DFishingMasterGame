using UnityEngine;

namespace Game.Inventory
{
    public abstract class InventoryItemBase
    {
        public abstract Sprite Icon { get; }
        public abstract string Name { get; }
    }
}