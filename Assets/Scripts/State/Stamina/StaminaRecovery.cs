using UnityEngine;

namespace Game.Stamina {
    public class StaminaRecovery : MonoBehaviour {
        // 未使用，但保留以供未來擴充
        public void DrinkCoffee(float amount) {
            StaminaController.Instance.ChangeStamina(amount);
        }
        public void RestFull() {
            var ctx = StaminaController.Instance;
            ctx.ChangeStamina(ctx.Max - ctx.Current);
        }
    }
}