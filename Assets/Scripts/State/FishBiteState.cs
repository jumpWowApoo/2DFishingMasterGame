using UnityEngine;
public class FishBiteState : IFishingState
{
    readonly FishingController fc; readonly float autoT,win; float t;
    public FishBiteState(FishingController fc,float auto,float win){ this.fc=fc; autoT=auto; this.win=win; }
    public void OnEnter(){ t=0; Debug.Log("魚漂下沉！"); }
    public void Tick()
    {
        t+=Time.deltaTime;
        if(Input.GetMouseButtonDown(0)) fc.BeginReel(t<=win,true);  // 需掛餌
        else if(t>=autoT)                fc.BeginReel(false,true); // 逾時失敗仍掛餌
    }
    public void OnExit(){}
}