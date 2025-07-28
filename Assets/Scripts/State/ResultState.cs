using UnityEngine;
using Game.UI;

/// <summary>顯示釣魚結果；成功時等玩家關閉面板再掛餌。</summary>
public class ResultState : IFishingState
{
    readonly FishingController fc;
    readonly bool success;
    readonly FishInfoPanel panel;
    readonly UIHub hub;

    public ResultState(FishingController fc, bool success,
        FishInfoPanel panel, UIHub hub)
    {
        this.fc      = fc;
        this.success = success;
        this.panel   = panel;
        this.hub     = hub;
    }

    public void OnEnter()
    {
        if (success)
        {
            Debug.Log($"panel={panel}, item={fc.CurrentFishItem}");
            Debug.Log($"玩家釣到：{fc.CurrentFishItem.data.fishName}");
            Debug.Log("成功釣魚");
            panel.Bind(fc.CurrentFishItem);
            OnPanelClosed();
            hub.ShowFishInfo();
        }
        else
        {
            Debug.Log("失敗");
            // 失敗立刻掛餌
            fc.SwitchTo(FishingController.StateID.Baiting);
        }
    }

    public void Tick() { }

    public void OnExit() => hub.HideFishInfo();

    /* 成功情況：面板關閉 → 回到掛餌 */
    void OnPanelClosed() => fc.SwitchTo(FishingController.StateID.Baiting);
}