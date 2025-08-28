using System.Collections.Generic;
using UnityEngine;

namespace Game.Consumables
{
    [CreateAssetMenu(menuName="Consumables/Consumable Data")]
    public class ConsumableData : ScriptableObject
    {
        [Header("基本")]
        public string itemId;
        public string itemName;
        [TextArea] public string description;
        public Sprite icon;
        [Min(0)] public int buyPrice = 10;

        [Header("使用效果（可多個）")]
        public List<ConsumableEffect> effects = new();

        public void Use(ConsumableContext ctx)
        {
            if (effects == null) return;
            foreach (var e in effects)
                if (e != null) e.Apply(ctx);
        }
    }
}