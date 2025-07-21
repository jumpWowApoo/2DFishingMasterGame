using System.Collections;
using UnityEngine;

/// <summary>
/// 控制「竿身」Animator，不含魚漂動畫
/// </summary>
[RequireComponent(typeof(Animator))]
public class RodAnimation : MonoBehaviour
{
    public enum Clip { Idle, Cast, Reel }

    [Header("Rod Animator State 名稱")]
    [SerializeField] string idleState = "Rod_Idle";
    [SerializeField] string castState = "Rod_Cast";
    [SerializeField] string reelState = "Rod_Reel";

    [Header("片段長度 (秒) 0=自動偵測")]
    [SerializeField] float idleLen = 0f;
    [SerializeField] float castLen = 0.5f;
    [SerializeField] float reelLen = 0.3f;

    Animator ani;
    public bool IsBusy { get; private set; }

    void Awake()
    {
        ani = GetComponent<Animator>();
        if (!ani.runtimeAnimatorController)
        {
            Debug.LogError($"[{name}] Animator 無 Controller！");
            enabled = false;
            return;
        }
        AutoLen(ref idleLen, idleState);
        AutoLen(ref castLen, castState);
        AutoLen(ref reelLen, reelState);
    }

    public void PlayIdle()
    {
        if (!IsBusy) ani.Play(idleState, 0, 0f);
    }

    public IEnumerator Play(Clip clip)
    {
        if (IsBusy) yield break;
        IsBusy = true;

        switch (clip)
        {
            case Clip.Cast:  yield return PlayState(castState, castLen);  break;
            case Clip.Reel:  yield return PlayState(reelState, reelLen);  break;
            default:         PlayIdle();                                  break;
        }
        IsBusy = false;
    }

    #region ─── helpers
    IEnumerator PlayState(string state, float len)
    {
        ani.Play(state, 0, 0f);
        yield return new WaitForSeconds(Mathf.Max(0.1f, len));
    }

    void AutoLen(ref float len, string state)
    {
        if (len > 0f) return;
        foreach (var c in ani.runtimeAnimatorController.animationClips)
            if (c.name == state) { len = c.length; break; }
    }
    #endregion
}