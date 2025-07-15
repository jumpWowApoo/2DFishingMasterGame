using UnityEngine;
using UnityEngine.UI;
public class FishingState : IFishingState
{
    readonly FishingController fc;
    readonly Vector2            rng;
    readonly Button             reelBtn;

    float timer;

    public FishingState(FishingController fc, Vector2 rng, Button reelBtn)
    {
        this.fc      = fc;
        this.rng     = rng;
        this.reelBtn = reelBtn;
    }

    public void OnEnter()
    {
        timer = Random.Range(rng.x, rng.y);
        reelBtn.onClick.AddListener(OnEarlyReel);
    }

    public void Tick()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
            fc.SwitchTo(FishingController.StateID.FishBite);
    }

    public void OnExit()
    {
        reelBtn.onClick.RemoveListener(OnEarlyReel);
    }

    /* 玩家在魚未咬時提早收竿 → 不掛餌 */
    void OnEarlyReel() => fc.BeginReel(false, false);
}