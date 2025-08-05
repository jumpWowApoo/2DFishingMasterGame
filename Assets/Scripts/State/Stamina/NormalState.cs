using Game.Common;

namespace Game.Stamina {
    public class NormalState : IState<StaminaController> {
        StaminaVisualModule  visual;
        public void Enter(StaminaController ctx) {visual = ctx.VisualModule; visual.SetNormal();}
        public void Tick(float dt) {}
        public void Exit() {}
    }
}