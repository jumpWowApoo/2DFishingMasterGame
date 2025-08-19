// SceneReturnContext.cs
namespace Game.Common
{
    public enum ResetLevel { None, Soft, Hard }

    public static class SceneReturnContext
    {
        public static ResetLevel Reset = ResetLevel.None;
    }

    public interface IResettable
    {
        void ResetForNewRound(ResetLevel level);
    }
}
