using UnityEngine;

public class BaitingState : IFishingState
{
    readonly FishingController fc;

    public BaitingState(FishingController fc) => this.fc = fc;

    public void OnEnter() { Debug.Log("請掛餌！"); }
    public void Tick()    { }
    public void OnExit()  { }
}
