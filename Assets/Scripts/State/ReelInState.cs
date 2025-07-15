using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 收竿：等待 0.3 秒（可替換成動畫），隱藏釣線、銷毀魚漂，
/// 依 needBait 決定回 Idle 或進掛餌流程。
/// </summary>
public class ReelInState : IFishingState
{
    readonly FishingController fc;
    readonly FishingLine       line;
    readonly GameObject        bobber;
    readonly bool              success;
    readonly bool              needBait;
    readonly Button            castBtn;
    readonly Button            reelBtn;

    public ReelInState(
        FishingController fc,
        FishingLine       line,
        GameObject        bobber,
        bool              success,
        bool              needBait,
        Button            castBtn,
        Button            reelBtn)
    {
        this.fc       = fc;
        this.line     = line;
        this.bobber   = bobber;
        this.success  = success;
        this.needBait = needBait;
        this.castBtn  = castBtn;
        this.reelBtn  = reelBtn;
    }

    public void OnEnter()
    {
        castBtn.gameObject.SetActive(false);
        reelBtn.gameObject.SetActive(true);
        fc.StartCoroutine(ReelFlow());
    }

    public void Tick() { }
    public void OnExit() { }

    IEnumerator ReelFlow()
    {
        yield return new WaitForSeconds(0.3f);   // 收竿動畫時長佔位

        line.Show(false);
        if (bobber)
            Object.Destroy(bobber);

        castBtn.gameObject.SetActive(true);
        reelBtn.gameObject.SetActive(false);

        // 將 needBait 帶回控制器
        fc.EndReel(success, needBait);
    }
}