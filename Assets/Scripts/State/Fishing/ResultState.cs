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
        this.fc = fc;
        this.success = success;
        this.panel = panel;
        this.hub = hub;
    }

    public void OnEnter()
    {
        if (success)
        {
            Debug.Log("成功");
            AudioHub.I.PlayRod(RodSfx.ResultSuccess);
            if (fc.CurrentFishItem != null)
                FishCrate.I.Add(fc.CurrentFishItem);
            panel.Bind(fc.CurrentFishItem);
            hub.ShowFishInfo();
            panel.Bind(fc.CurrentFishItem, onClose: () =>
            {
                fc.SwitchTo(FishingController.StateID.Baiting);
            });
        }
        else
        {
            Debug.Log("失敗");
            AudioHub.I.PlayRod(RodSfx.ResultFail);
            fc.SwitchTo(FishingController.StateID.Baiting);
        }
    }

    public void Tick()
    {
    }

    public void OnExit()
    {
    }
}