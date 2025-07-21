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
    readonly RodAnimation rodAnim;
    BobberAnimation bobAnim;

    public CastingState(FishingController fc, Transform rodTip, Transform target, GameObject prefab,
        BobberMotion motion, FishingLine line,RodAnimation rodAnim, Button cast, Button reel)
    {
        this.fc = fc;
        this.rodTip = rodTip;
        this.target = target;
        this.prefab = prefab;
        this.motion = motion;
        this.line = line;
        castBtn = cast;
        reelBtn = reel;
        this.rodAnim = rodAnim;
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
        Coroutine castAnimCo = null;
        if (rodAnim)
        {
            castAnimCo = fc.StartCoroutine(rodAnim.Play(RodAnimation.Clip.Cast));
        }

        bob = Object.Instantiate(prefab, rodTip.position, rodTip.rotation);
         bobAnim = bob.GetComponent<BobberAnimation>() ??
                      bob.GetComponentInChildren<BobberAnimation>(true);
        if (!bobAnim)
        {
            Debug.LogError($"[Casting] {prefab.name} 內完全找不到 BobberAnimation！");
            yield break;
        }
        bobAnim.PlayIdle();
        fc.SetBobber(bob);
        fc.FishingStateRef.SetBobber(bobAnim);
        fc.StartCoroutine(bobAnim.Play(BobberAnimation.Clip.Idle));
        fc.Line.EnableSag(true);
        line.SetTargets(rodTip, fc.BobberAttachPoint);
        line.Show(true);
        yield return motion.MoveTo(bob.transform, target.position);
        fc.SwitchTo(FishingController.StateID.Fishing);
    }
}