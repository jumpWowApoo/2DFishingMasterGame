using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIHub : MonoBehaviour
{
    public static UIHub Instance { get; private set; }

    [Header("Windows")]
    [SerializeField] GameObject fishInfoWindow;
    [SerializeField] GameObject inventoryWindow;
    [SerializeField] GameObject missionWindow;
    [SerializeField] GameObject AudioWindow;
    [SerializeField] GameObject BarWindow;

    [Header("拖影父層 (DragLayer)")]
    [SerializeField] RectTransform dragLayer;
    public RectTransform DragLayer => dragLayer;

    [Header("Scenes (公開以便日後修改)")]
    [Tooltip("返回主選單時要載入的場景名稱")]
    public string mainMenuSceneName = "MainMenu";

    void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ToggleAudio();
    }

    public void CloseAll()
    {
        CloseFishInfo(); // 走會觸發 onClose 的流程
        if (inventoryWindow) inventoryWindow.SetActive(false);
        if (missionWindow)   missionWindow.SetActive(false);
        if (AudioWindow)     AudioWindow.SetActive(false);
        if (BarWindow)       BarWindow.SetActive(false);
    }

    public void ShowFishInfo()
    {
        if (fishInfoWindow) fishInfoWindow.SetActive(true);
    }

    /// <summary>關閉魚視窗：優先走 FishInfoPanel.Close() 以觸發 onClose。</summary>
    public void CloseFishInfo()
    {
        if (!fishInfoWindow) return;
        var panel = fishInfoWindow.GetComponent<Game.UI.FishInfoPanel>();
        if (panel != null) panel.Close();
        else fishInfoWindow.SetActive(false);
    }

    // 舊 API：保留但改走 CloseFishInfo（避免遺漏 onClose）
    public void HideFishInfo() => CloseFishInfo();

    public void ToggleBag()
    {
        if (inventoryWindow) inventoryWindow.SetActive(!inventoryWindow.activeSelf);
    }

    public void HideMission()
    {
        if (missionWindow) missionWindow.SetActive(false);
    }

    public void ToggleMission()
    {
        if (missionWindow) missionWindow.SetActive(!missionWindow.activeSelf);
    }

    public void ToggleAudio()
    {
        if (AudioWindow) AudioWindow.SetActive(!AudioWindow.activeSelf);
    }

    /// <summary>
    /// 返回主選單（Single 載入）。可綁在「返回主選單」按鈕。
    /// </summary>
    public void ReturnToMainMenu()
    {
        if (string.IsNullOrEmpty(mainMenuSceneName))
        {
            Debug.LogError("[UIHub] mainMenuSceneName 未設定，請在 Inspector 指定主選單場景名稱。");
            return;
        }

        // 可選：恢復時間縮放，避免從暫停狀態回主選單
        if (Time.timeScale == 0f) Time.timeScale = 1f;

        CloseAll(); // 回主選單前先把當前 UI 關掉（可選）
        SceneManager.LoadScene(mainMenuSceneName, LoadSceneMode.Single);
    }
}
