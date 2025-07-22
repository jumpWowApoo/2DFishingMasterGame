using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 判斷釣魚成功與失敗
/// </summary>
public class ResultState : IFishingState
{
    readonly FishingController fc;
    readonly bool success;
    readonly Button castBtn, reelBtn;
    private readonly RodAnimation rodAnim;
    

    public ResultState(FishingController fc, bool success, Button cast, Button reel,RodAnimation rodAnim)
    {
        this.fc = fc;
        this.success = success;
        castBtn = cast;
        reelBtn = reel;
        this.rodAnim = rodAnim;
    }

    public void OnEnter()
    {
        if (success)
        {
            success_fish();
        }
        else
        {
            Debug.Log("釣魚失敗");
        }
        fc.SwitchTo(FishingController.StateID.Baiting);
    }

    public void Tick()
    {
    }

    public void OnExit()
    {
    }

    private void success_fish()
    {
        List<FishData> pool = fc.fishDB.fishes;
        FishData fish = FishPicker.PickRandomFish(pool);
        Debug.Log($"玩家釣到：{fish.fishName}");
    }
}