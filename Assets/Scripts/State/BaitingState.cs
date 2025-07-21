using System.Collections;
using UnityEngine;

/// <summary>
/// 無動畫版掛餌：等待 fixedDelay 秒後自動回 Idle
/// </summary>
public class BaitingState : IFishingState
{
    readonly FishingController fc;
    readonly float fixedDelay = 1.0f;   // 掛餌所需時間（秒）
    private readonly RodAnimation rodAnim;

    public BaitingState(FishingController fc,RodAnimation rodAnim)
    {
        this.fc = fc;
        this.rodAnim = rodAnim;
    }

    public void OnEnter()
    {
        fc.StartCoroutine(WaitAndReturn());
    }

    public void Tick() { }
    public void OnExit() { }

    IEnumerator WaitAndReturn()
    {
        yield return new WaitForSeconds(fixedDelay);
        fc.SwitchTo(FishingController.StateID.Idle);
    }
}