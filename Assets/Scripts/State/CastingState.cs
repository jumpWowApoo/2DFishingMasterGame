using System.Collections;
using UnityEngine;

public class CastingState : IFishingState
{
    readonly FishingController fc;
    readonly Transform rodTip, target;
    readonly GameObject prefab;
    readonly BobberMotion motion;
    readonly FishingLine line;

    GameObject bobber;

    public CastingState(FishingController fc,
        Transform rodTip,
        Transform target,
        GameObject prefab,
        BobberMotion motion,
        FishingLine line)
    {
        this.fc = fc; this.rodTip = rodTip; this.target = target;
        this.prefab = prefab; this.motion = motion; this.line = line;
    }

    public void OnEnter() => fc.StartCoroutine(Flow());
    public void Tick()    { }
    public void OnExit()  { }

    IEnumerator Flow()
    {
        bobber = Object.Instantiate(prefab, rodTip.position, Quaternion.identity);

        line.SetTargets(rodTip, bobber.transform);
        line.Show(true);

        yield return motion.MoveTo(bobber.transform, target.position);

        Debug.Log("釣魚中");
        fc.SwitchTo(FishingController.StateID.Baiting);
    }

    public void DestroyBobber()
    {
        if (bobber) Object.Destroy(bobber);
        line.Show(false);
    }
}