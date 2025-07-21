using UnityEngine;

public class FishBiteState : IFishingState
{
    readonly FishingController fc;
    readonly float autoT, win;
    float t;
    private readonly RodAnimation rodAnim;

    public FishBiteState(FishingController fc, float auto, float win, RodAnimation rodAnim)
    {
        this.fc = fc;
        autoT = auto;
        this.win = win;
        this.rodAnim = rodAnim;
    }

    public void OnEnter()
    {
        t = 0;
        Debug.Log("魚漂下沉！");
        var bobAnim = fc.CurrentBobber.GetComponent<BobberAnimation>();
        fc.StartCoroutine(bobAnim.Play(BobberAnimation.Clip.Sink));
    }

    public void Tick()
    {
        t += Time.deltaTime;
        if (Input.GetMouseButtonDown(0)) fc.BeginReel(t <= win, true); // 需掛餌
        else if (t >= autoT) fc.BeginReel(false, true); // 逾時失敗仍掛餌
    }

    public void OnExit()
    {
    }
}