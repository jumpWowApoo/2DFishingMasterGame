using UnityEngine;
using UnityEngine.UI;

public class FishBiteState : IFishingState
{
    readonly FishingController fc;
    readonly float autoT, win;
    float t;
    readonly RodAnimation rodAnim;
    readonly Button reelBut;
    bool subscribed;

    public FishBiteState(FishingController fc, float auto, Button reel, float win, RodAnimation rodAnim)
    {
        this.fc = fc;
        autoT = auto;
        this.win = win;
        this.rodAnim = rodAnim;
        this.reelBut = reel;
    }

    public void OnEnter()
    {
        AudioHub.I.PlayRod(RodSfx.Bite);
        AudioHub.I.PlayBobber(BobberSfx.Bite);
        t = 0f;
        Debug.Log("魚漂下沉！");

        var bobAnim = fc.CurrentBobber ? fc.CurrentBobber.GetComponent<BobberAnimation>() : null;
        if (bobAnim) fc.StartCoroutine(bobAnim.Play(BobberAnimation.Clip.Sink));

        // ★ 只在進入時綁一次
        if (!subscribed)
        {
            reelBut.onClick.AddListener(OnReelClicked);
            subscribed = true;
        }
        reelBut.interactable = true; // 可按
    }

    public void Tick()
    {
        t += Time.deltaTime;

        // ★ 逾時自動失敗（先解綁再進下一步）
        if (t >= autoT)
        {
            Debug.Log("下沉後掉魚失敗");
            SafeUnsubscribe();
            reelBut.interactable = false;
            fc.BeginReel(false, true);
        }
    }

    public void OnExit()
    {
        SafeUnsubscribe();
        reelBut.interactable = true; // 歸還控制權
    }

    void OnReelClicked()
    {
        // 先解綁避免同幀/多次觸發
        SafeUnsubscribe();
        reelBut.interactable = false;

        bool isSuccess = (t <= win);
        Debug.Log(isSuccess ? "下沉後掉魚成功" : "下沉後掉魚失敗(太慢)");
        fc.BeginReel(isSuccess, true);
    }

    void SafeUnsubscribe()
    {
        if (subscribed)
        {
            reelBut.onClick.RemoveListener(OnReelClicked);
            subscribed = false;
        }
    }
}