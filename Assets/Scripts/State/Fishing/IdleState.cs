using UnityEngine.UI;

public class IdleState : IFishingState
{
    readonly RodAnimation rodAnim;
    readonly FishingController fc;
    readonly Button castBtn, reelBtn;

    public IdleState(FishingController fc, Button cast, Button reel,RodAnimation rodAnim)
    {
        this.fc = fc;
        castBtn = cast;
        reelBtn = reel;
        this.rodAnim = rodAnim;
    }

    public void OnEnter()
    {
        castBtn.gameObject.SetActive(true);
        reelBtn.gameObject.SetActive(false);
        castBtn.onClick.AddListener(OnCast);
    }

    public void Tick()
    {
    }

    public void OnExit()
    {
        if (rodAnim) rodAnim.PlayIdle();
        castBtn.onClick.RemoveListener(OnCast);
    }

    void OnCast() => fc.SwitchTo(FishingController.StateID.Casting);
}