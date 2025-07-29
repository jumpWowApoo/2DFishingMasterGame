using UnityEngine;
using UnityEngine.UI;

public class FishingState : IFishingState
{
    public float WaitTotal { get; private set; } // 本輪總秒數
    public float WaitRemaining => _timer; // 尚餘秒
    public float WaitElapsed => WaitTotal - _timer; // 已過秒

    readonly FishingController fc;
    readonly Vector2 rng;
    readonly Button reelBtn;
    private readonly RodAnimation rodAnim;
    private BobberAnimation bobAnim;
    public void SetBobber(BobberAnimation anim) => bobAnim = anim;

    float _timer;

    public FishingState(FishingController fc, Vector2 rng, Button reelBtn, RodAnimation rodAnim)
    {
        this.fc = fc;
        this.rng = rng;
        this.reelBtn = reelBtn;
        this.rodAnim = rodAnim;
    }

    public void OnEnter()
    {
        _timer = Random.Range(rng.x, rng.y);
        WaitTotal = _timer;
        reelBtn.onClick.AddListener(OnEarlyReel);
        var bobAnim = fc.CurrentBobber.GetComponent<BobberAnimation>();
        fc.StartCoroutine(bobAnim.Play(BobberAnimation.Clip.Float));
    }

    public void Tick()
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0)
            fc.SwitchTo(FishingController.StateID.FishBite);
    }

    public void OnExit()
    {
        reelBtn.onClick.RemoveListener(OnEarlyReel);
    }

    /* 玩家在魚未咬時提早收竿 → 不掛餌 */
    void OnEarlyReel() => fc.BeginReel(false, false);
}