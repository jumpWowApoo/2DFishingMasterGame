using Game.Common;

namespace Game.Stamina {
    public class WearyHighState : IState<StaminaController> {
        BlinkAnimationModule blink;
        public void Enter(StaminaController ctx) {
            blink = ctx.BlinkModule;
            blink.SetBlink(5f,1f);
        }
        public void Tick(float dt) {}
        public void Exit() {
            blink.SetBlink(0f,1f);
        }
    }
}