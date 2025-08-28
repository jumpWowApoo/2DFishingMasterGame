using UnityEngine;

namespace Game.Consumables
{
    public abstract class ConsumableEffect : ScriptableObject
    {
        public abstract void Apply(ConsumableContext ctx);
    }
}