using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class IdleState : IFishingState
{
    readonly FishingController fc;
    readonly Button castBtn;

    public IdleState(FishingController fc, Button btn)
    { this.fc = fc; castBtn = btn; }

    public void OnEnter() => castBtn.onClick.AddListener(OnClick);
    public void Tick()    { }
    public void OnExit()  => castBtn.onClick.RemoveListener(OnClick);

    void OnClick() => fc.SwitchTo(FishingController.StateID.Casting);
}
