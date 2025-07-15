using UnityEngine;
using UnityEngine.UI;

public class BaitUI : MonoBehaviour
{
    [SerializeField] Button baitButton;
    [SerializeField] FishingController controller;

    void Awake()
    {
        baitButton.onClick.AddListener(() =>
        {
            // 玩家掛餌後回到 Idle
            controller.SwitchTo(FishingController.StateID.Idle);
            gameObject.SetActive(false);
        });
        gameObject.SetActive(false);  // 預設隱藏
    }

    public void Show() => gameObject.SetActive(true);
}