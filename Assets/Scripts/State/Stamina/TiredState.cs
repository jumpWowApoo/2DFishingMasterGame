using Game.Common;

namespace Game.Stamina {
    public class TiredState : IState<StaminaController> {
        BlinkAnimationModule blink;
        StaminaVisualModule  visual;
        public void Enter(StaminaController ctx) {
            blink = ctx.BlinkModule;
            visual = ctx.VisualModule;
            blink.SetBlink(12f,1f);
            visual.SetTired();
        }
        public void Tick(float dt) {}
        public void Exit() {
        }
    }
}