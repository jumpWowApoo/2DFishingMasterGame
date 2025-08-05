using Game.Common;

namespace Game.Stamina {
    public class WearyLowState : IState<StaminaController> {
        BlinkAnimationModule blink;
        public void Enter(StaminaController ctx) {
            blink = ctx.BlinkModule;
            blink.SetBlink(5f,2f);
        }
        public void Tick(float dt) {}
        public void Exit() {
            blink.SetBlink(0f,0f);
        }
    }
}