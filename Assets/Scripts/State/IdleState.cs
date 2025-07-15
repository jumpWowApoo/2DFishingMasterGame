using UnityEngine.UI;

public class IdleState : IFishingState
{
    readonly FishingController fc;
    readonly Button castBtn, reelBtn;

    public IdleState(FishingController fc, Button cast, Button reel)
    {
        this.fc = fc;
        castBtn = cast;
        reelBtn = reel;
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

    public void OnExit() => castBtn.onClick.RemoveListener(OnCast);
    void OnCast() => fc.SwitchTo(FishingController.StateID.Casting);
}