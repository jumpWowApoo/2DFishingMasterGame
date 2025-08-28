using UnityEngine;
using UnityEngine.SceneManagement;

public class UIHub_Start : MonoBehaviour
{
    [Header("Scenes (公開以便日後修改)")]
    [Tooltip("按『開始遊戲』要載入的場景名稱")]
    public string startSceneName = "Scene_A";

    [Header("Panels")]
    [Tooltip("主選單面板（包含：開始／離開）")]
    [SerializeField] GameObject startMenuPanel;

    [Header("Behavior")]
    [Tooltip("進入場景時是否自動顯示主選單")]
    [SerializeField] bool showStartMenuOnAwake = true;

    void Awake()
    {
        if (showStartMenuOnAwake) ShowStartMenu();
    }

    // ===== Buttons =====
    // 「開始遊戲」按鈕綁這個
    public void OnClickStartGame()
    {
        if (string.IsNullOrEmpty(startSceneName))
        {
            Debug.LogError("[UIHub_Start] startSceneName 是空的，請在 Inspector 設定要載入的場景名稱。");
            return;
        }
        SceneManager.LoadScene(startSceneName, LoadSceneMode.Single);
    }

    // 「離開遊戲」按鈕綁這個
    public void OnClickQuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ===== Helpers =====
    public void ShowStartMenu()
    {
        if (startMenuPanel) startMenuPanel.SetActive(true);
    }
}