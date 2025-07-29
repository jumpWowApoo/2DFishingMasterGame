using UnityEngine;

namespace Game.Stamina {
    public class StaminaDepleter : MonoBehaviour {
        [Header("自動衰減速率")]
        [SerializeField] float decayRate = 1f;

        void Update() {
            StaminaController.Instance.ChangeStamina(-decayRate * Time.deltaTime);
        }
    }
}