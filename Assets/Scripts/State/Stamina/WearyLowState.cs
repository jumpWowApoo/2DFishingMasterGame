using Game.Common;

namespace Game.Stamina {
    public class WearyLowState : IState<StaminaController> {
        BlinkAnimationModule blink;
        StaminaVisualModule  visual;
        public void Enter(StaminaController ctx) {
            blink = ctx.BlinkModule;
            visual = ctx.VisualModule;
            blink.SetBlink(5f,2f);
            visual.SetWearyLow();
        }
        public void Tick(float dt) {}
        public void Exit() {
            blink.SetBlink(0f,0f);
        }
    }
}