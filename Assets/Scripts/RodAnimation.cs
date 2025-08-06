using System.Collections;
using UnityEngine;

/// <summary>
/// 控制「竿身」Animator。<br/>
/// - 可把本腳本 **與 Animator 放在同一個子物件**<br/>
/// - 也可把腳本放在父物件 → Inspector 指定 targetAnimator 或自動往下找
/// </summary>
[DisallowMultipleComponent]                        // ✔ 不強制一定有 Animator
public class RodAnimation : MonoBehaviour
{
    public enum Clip { Idle, Cast, Reel }

    [Header("Animator 來源 (留空 = 自動尋找)")]
    [SerializeField] Animator targetAnimator;      // ★ 新增：明確指定

    [Header("Animator State 名稱")]
    [SerializeField] string idleState  = "Rod_Idle";
    [SerializeField] string castState  = "Rod_Cast";
    [SerializeField] string reelState  = "Rod_Reel";

    [Header("片段長度 (秒) 0=自動偵測")]
    [SerializeField] float idleLen = 0f;
    [SerializeField] float castLen = 0.5f;
    [SerializeField] float reelLen = 0.3f;

    Animator ani;
    public bool IsBusy { get; private set; }

    void Awake()
    {
        // ① 優先用 Inspector 指定
        ani = targetAnimator ??
              // ② 再找同物件
              GetComponent<Animator>() ??
              // ③ 最後往子物件尋找
              GetComponentInChildren<Animator>(true);

        if (!ani)
        {
            Debug.LogError($"[{name}] 找不到 Animator！", this);
            enabled = false;
            return;
        }

        AutoLen(ref idleLen, idleState);
        AutoLen(ref castLen, castState);
        AutoLen(ref reelLen, reelState);
    }

    public void PlayIdle() { if (!IsBusy) ani.Play(idleState, 0, 0f); }

    public IEnumerator Play(Clip clip)
    {
        if (IsBusy) yield break;
        IsBusy = true;

        switch (clip)
        {
            case Clip.Cast:  yield return PlayState(castState,  castLen); break;
            case Clip.Reel:  yield return PlayState(reelState,  reelLen); break;
            default:         PlayIdle();                                   break;
        }
        IsBusy = false;
    }

    /* ─ helpers ─ */
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
}
