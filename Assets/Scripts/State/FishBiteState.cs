using UnityEngine;
using UnityEngine.UI;

public class FishBiteState : IFishingState
{
    readonly FishingController fc;
    readonly float autoT, win;
    float t;
    private readonly RodAnimation rodAnim;
    private readonly Button reelBut;

    public FishBiteState(FishingController fc, float auto, Button reel,float win, RodAnimation rodAnim)
    {
        this.fc = fc;
        autoT = auto;
        this.win = win;
        this.rodAnim = rodAnim;
        this.reelBut = reel;
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
        
        reelBut.onClick.AddListener(OnReelClicked);

        // 超過時間自動失敗並掛餌
        if (t >= autoT)
        {
            // 移除監聽，避免後續重複觸發
            reelBut.onClick.RemoveListener(OnReelClicked);
            fc.BeginReel(false, true);
        }
    }

    public void OnExit()
    {
        // ★ 在離開狀態時一定要移除監聽
        reelBut.onClick.RemoveListener(OnReelClicked);
    }

    private void OnReelClicked()
    {
        reelBut.onClick.RemoveListener(OnReelClicked);
        fc.BeginReel(t <= win, true);
    }
}
