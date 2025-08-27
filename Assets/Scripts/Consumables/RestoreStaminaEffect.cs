using UnityEngine;
using Game.Stamina; // 你現有的體力控制器

namespace Game.Consumables
{
    [CreateAssetMenu(menuName="Consumables/Effects/Restore Stamina")]
    public class RestoreStaminaEffect : ConsumableEffect
    {
        [Tooltip("回復的體力量（絕對值）")]
        public float amount = 20f;

        public override void Apply(ConsumableContext ctx)
        {
            var sc = StaminaController.Instance;
            if (sc != null) sc.ChangeStamina(+amount);
        }
    }
}