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
    readonly FishingLine line;
    readonly GameObject bobber;
    readonly bool success;
    readonly bool needBait;
    readonly Button castBtn;
    readonly Button reelBtn;
    readonly RodAnimation rodAnim;

    public ReelInState(
        FishingController fc,
        FishingLine line,
        GameObject bobber,
        bool success,
        bool needBait,
        Button castBtn,
        Button reelBtn,
        RodAnimation rodAnim)
    {
        this.fc = fc;
        this.line = line;
        this.bobber = bobber;
        this.success = success;
        this.needBait = needBait;
        this.castBtn = castBtn;
        this.reelBtn = reelBtn;
        this.rodAnim = rodAnim;
    }

    public void OnEnter()
    {
        fc.StartCoroutine(ReelFlow());
    }

    public void Tick()
    {
    }

    public void OnExit()
    {
    }

    IEnumerator ReelFlow()
    {
        Coroutine castAnimCo = null;
        if (rodAnim)
            if (rodAnim)
                yield return fc.StartCoroutine(rodAnim.Play(RodAnimation.Clip.Reel));
            else
                yield return new WaitForSeconds(0.3f); // 沒動畫才用固定延遲

        line.Show(false);
        if (bobber)
            Object.Destroy(bobber);

        castBtn.gameObject.SetActive(false);
        reelBtn.gameObject.SetActive(false);
        fc.Line.EnableSag(false);

        // 將 needBait 帶回控制器
        fc.EndReel(success, needBait);
    }
}