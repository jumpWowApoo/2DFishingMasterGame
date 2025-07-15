using UnityEngine;
using UnityEngine.UI;

public class ResultState : IFishingState
{
    readonly FishingController fc;
    readonly bool success;
    readonly Button castBtn, reelBtn;

    public ResultState(FishingController fc, bool success, Button cast, Button reel)
    {
        this.fc = fc;
        this.success = success;
        castBtn = cast;
        reelBtn = reel;
    }

    public void OnEnter()
    {
        Debug.Log(success ? "釣魚成功！" : "釣魚失敗…");
        fc.SwitchTo(FishingController.StateID.Baiting);
    }

    public void Tick()
    {
    }

    public void OnExit()
    {
    }
}