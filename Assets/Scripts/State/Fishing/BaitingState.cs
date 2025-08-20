using System.Collections;
using Game.UI;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 無動畫版掛餌：等待 fixedDelay 秒後自動回 Idle
/// </summary>
public class BaitingState : IFishingState
{
    readonly FishingController fc;
    readonly float fixedDelay = 1.0f; // 掛餌所需時間（秒）
    private readonly RodAnimation rodAnim;
    readonly Button castBtn;
    readonly Button reelBtn;
    readonly FishInfoPanel panel;

    public BaitingState(FishingController fc, RodAnimation rodAnim, Button castBtn, Button reelBtn,FishInfoPanel panel)
    {
        this.fc = fc;
        this.rodAnim = rodAnim;
        this.castBtn = castBtn;
        this.reelBtn = reelBtn;
        this.panel = panel;
    }

    public void OnEnter()
    {
        AudioHub.I.PlayRod(RodSfx.Bait);
        fc.StartCoroutine(WaitAndReturn());
    }

    public void Tick()
    {
        
    }

    public void OnExit()
    {
    }

    IEnumerator WaitAndReturn()
    {
        yield return new WaitForSeconds(fixedDelay);
        fc.SwitchTo(FishingController.StateID.Idle);
    }
}