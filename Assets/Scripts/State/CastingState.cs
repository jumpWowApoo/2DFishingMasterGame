using System.Collections; 
using UnityEngine;
using UnityEngine.UI;

public class CastingState : IFishingState
{
    readonly FishingController fc;
    readonly Transform rodTip, target;
    readonly GameObject prefab;
    readonly BobberMotion motion;
    readonly FishingLine line;
    readonly Button castBtn, reelBtn;
    GameObject bob;

    public CastingState(FishingController fc, Transform rodTip, Transform target, GameObject prefab,
        BobberMotion motion, FishingLine line, Button cast, Button reel)
    {
        this.fc = fc;
        this.rodTip = rodTip;
        this.target = target;
        this.prefab = prefab;
        this.motion = motion;
        this.line = line;
        castBtn = cast;
        reelBtn = reel;
    }

    public void OnEnter()
    {
        castBtn.gameObject.SetActive(false);
        reelBtn.gameObject.SetActive(true);
        fc.StartCoroutine(Flow());
    }

    public void Tick()
    {
    }

    public void OnExit()
    {
    }

    IEnumerator Flow()
    {
        bob = Object.Instantiate(prefab, rodTip.position, Quaternion.identity);
        fc.SetBobber(bob);
        line.SetTargets(rodTip, bob.transform);
        line.Show(true);
        yield return motion.MoveTo(bob.transform, target.position);
        fc.SwitchTo(FishingController.StateID.Fishing);
    }
}