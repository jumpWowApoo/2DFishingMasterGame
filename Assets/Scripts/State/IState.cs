namespace Game.Common {
    /// <summary>
    /// 通用狀態介面，支援多個 Context
    /// 釣魚系統並非用此介面
    /// </summary>
    public interface IState<TContext> {
        void Enter(TContext ctx);
        void Tick(float deltaTime);
        void Exit();
    }
}