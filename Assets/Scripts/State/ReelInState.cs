using System.Collections;
using UnityEngine;

/// <summary>
/// 收竿：播放動畫（這裡用等待取代）→ 隱藏釣線 → 銷毀魚標
/// 完成後呼叫控制器，切到 Success / Fail 狀態
/// </summary>
public class ReelInState : IFishingState
{
    readonly FishingController fc;
    readonly FishingLine       line;
    readonly GameObject        bobber;
    readonly bool              success;   // true = 釣到魚

    public ReelInState(FishingController fc, FishingLine line,
        GameObject bobber, bool success)
    {
        this.fc      = fc;
        this.line    = line;
        this.bobber  = bobber;
        this.success = success;
    }

    public void OnEnter() => fc.StartCoroutine(ReelFlow());
    public void Tick()    { }
    public void OnExit()  { }

    IEnumerator ReelFlow()
    {
        yield return new WaitForSeconds(0.3f);

        /* 關閉線、銷毀魚標 */
        line.Show(false);
        if (bobber) Object.Destroy(bobber);

        /* 告知主控制器結果 */
        fc.BeginReel(line, bobber, success);
    }
}