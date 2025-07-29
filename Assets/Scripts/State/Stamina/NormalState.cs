using Game.Common;

namespace Game.Stamina {
    public class NormalState : IState<StaminaController> {
        public void Enter(StaminaController ctx) {}
        public void Tick(float dt) {}
        public void Exit() {}
    }
}