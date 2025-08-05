using Game.Common;

namespace Game.Stamina {
    public class TiredState : IState<StaminaController> {
        BlinkAnimationModule blink;
        public void Enter(StaminaController ctx) {
            blink = ctx.BlinkModule;
            blink.SetBlink(12f,1f);
        }
        public void Tick(float dt) {}
        public void Exit() {
        }
    }
}