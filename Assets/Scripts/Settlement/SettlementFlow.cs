using UnityEngine.SceneManagement;

public static class SettlementFlow
{
    // 依你的結算場景名稱調整（你現在是 GameOverScene）
    private static readonly string SettlementSceneName = "GameOverScene";

    // 進結算前的遊戲場景名
    private static string _prevScene;

    /// <summary>切換到結算場景（Single）。</summary>
    public static void OpenSettlement()
    {
        _prevScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(SettlementSceneName, LoadSceneMode.Single);
    }

    /// <summary>從結算回到剛才的遊戲場景（Single → 重載＝初始化）。</summary>
    public static void ReturnToGame()
    {
        if (string.IsNullOrEmpty(_prevScene))
        {
            // 後備場景名（請改成你的主要遊戲場景）
            _prevScene = "S1";
        }
        SceneManager.LoadScene(_prevScene, LoadSceneMode.Single);
    }
}