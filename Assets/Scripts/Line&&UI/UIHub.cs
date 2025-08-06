using System;
using UnityEngine;

/// <summary>集中開關各 UI 面板。</summary>
public class UIHub : MonoBehaviour
{
    public static UIHub Instance { get; private set; }

    [SerializeField] GameObject fishInfoWindow;
    [SerializeField] GameObject inventoryWindow;
    [SerializeField] GameObject missionWindow; 
    
    [Header("拖影父層 (DragLayer)")]
    [SerializeField] RectTransform dragLayer; 
    
    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }


    public void CloseAll()
    {
        fishInfoWindow.SetActive(false);
        inventoryWindow.SetActive(false);
        missionWindow.SetActive(false);
    }

    public void ShowFishInfo() => fishInfoWindow.SetActive(true);
    public void HideFishInfo() => fishInfoWindow.SetActive(false);
    public void ToggleBag() => inventoryWindow.SetActive(!inventoryWindow.activeSelf);
    public void HideMission()  => missionWindow.SetActive(false);
    public void ToggleMission() =>
        missionWindow.SetActive(!missionWindow.activeSelf);
    
    public RectTransform DragLayer => dragLayer;
}