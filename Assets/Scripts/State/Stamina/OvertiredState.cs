using Game.Common;
using UnityEngine.SceneManagement;

namespace Game.Stamina {
    public class OvertiredState : IState<StaminaController> {
        public void Enter(StaminaController ctx) {
            SceneManager.LoadScene("GameOverScene");
        }
        public void Tick(float dt) {}
        public void Exit() {}
    }
}