using System;
using UnityEngine;

/// <summary>集中開關各 UI 面板。</summary>
public class UIHub : MonoBehaviour
{
    [SerializeField] GameObject fishInfoWindow;
    [SerializeField] GameObject inventoryWindow;

    private void Start()
    {
       // HideFishInfo();
       // inventoryWindow.SetActive(false);
    }

    public void ShowFishInfo() => fishInfoWindow.SetActive(true);
    public void HideFishInfo() => fishInfoWindow.SetActive(false);
    public void ToggleBag() => inventoryWindow.SetActive(!inventoryWindow.activeSelf);
}