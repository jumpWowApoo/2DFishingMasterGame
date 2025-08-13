using UnityEngine;
using UnityEngine.SceneManagement;

public static class SettlementFlow
{
    // 換成你的結算場景名
    static string settlementSceneName = "GameOverScene";
    static bool isOpen;

    public static void OpenSettlement()
    {
        if (isOpen) return;
        isOpen = true;
        Time.timeScale = 0f; // 可選：暫停遊戲
        SceneManager.LoadScene(settlementSceneName, LoadSceneMode.Additive);
    }

    public static void ReturnToGame()
    {
        if (!isOpen) return;
        isOpen = false;
        Time.timeScale = 1f;
        SceneManager.UnloadSceneAsync(settlementSceneName);
    }
}