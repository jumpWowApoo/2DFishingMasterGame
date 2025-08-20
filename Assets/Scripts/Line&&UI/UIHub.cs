using System;
using UnityEngine;

public class UIHub : MonoBehaviour
{
    public static UIHub Instance { get; private set; }

    [SerializeField] GameObject fishInfoWindow;
    [SerializeField] GameObject inventoryWindow;
    [SerializeField] GameObject missionWindow;
    [SerializeField] GameObject shopWindow;
    [SerializeField] GameObject AudioWindow;

    [Header("拖影父層 (DragLayer)")]
    [SerializeField] RectTransform dragLayer;
    public RectTransform DragLayer => dragLayer;

    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
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
        if (shopWindow)      shopWindow.SetActive(false);
        if (AudioWindow)     AudioWindow.SetActive(false);
    }

    public void ShowFishInfo() { if (fishInfoWindow) fishInfoWindow.SetActive(true); }

    /// <summary>關閉魚視窗：優先走 FishInfoPanel.Close() 以觸發 onClose</summary>
    public void CloseFishInfo()
    {
        if (!fishInfoWindow) return;
        var panel = fishInfoWindow.GetComponent<Game.UI.FishInfoPanel>();
        if (panel != null) panel.Close();
        else fishInfoWindow.SetActive(false);
    }

    // 舊 API：保留但改走 CloseFishInfo（避免遺漏 onClose）
    public void HideFishInfo() => CloseFishInfo();

    public void ToggleBag()     { if (inventoryWindow) inventoryWindow.SetActive(!inventoryWindow.activeSelf); }
    public void HideMission()   { if (missionWindow)   missionWindow.SetActive(false); }
    public void ToggleMission() { if (missionWindow)   missionWindow.SetActive(!missionWindow.activeSelf); }
    public void ToggleShop()    { if (shopWindow)      shopWindow.SetActive(!shopWindow.activeSelf); }
    public void ToggleAudio()   { if (AudioWindow)     AudioWindow.SetActive(!AudioWindow.activeSelf); }
}
