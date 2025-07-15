using System.Collections;
using UnityEngine;

/// <summary>
/// 不用 Animator Trigger，改直接呼叫 Animator.Play(stateName)。
/// clipLength 可在 Inspector 設定，或自動從 Animator 讀取。<br/>
/// 外部：
///   • PlayIdle()            – 立即 Idle
///   • yield return Play(Clip.Cast) – 播 Cast 動畫並等待結束
///   • yield return PlayState("Jump", 0.8f) – 播自訂 State
/// </summary>
[RequireComponent(typeof(Animator))]
public class RodAnimation : MonoBehaviour
{
    public enum Clip
    {
        Idle,
        Cast,
        Reel
    }

    [Header("Animator State 名稱")] [SerializeField]
    string idleState = "Rod_Idle";

    [SerializeField] string castState = "Rod_Cast";
    [SerializeField] string reelState = "Rod_Reel";

    [Header("各片段長度 (秒) ─ 若為 0 會自動從 AnimatorClip 讀取")] [SerializeField]
    float idleLen = 0f;

    [SerializeField] float castLen = 0.5f;
    [SerializeField] float reelLen = 0.3f;

    Animator anim;
    public bool IsBusy { get; private set; }

    void Start()
    {
        anim = GetComponent<Animator>();
        if (!anim.runtimeAnimatorController)
        {
            Debug.LogError($"[{name}] Animator 沒有指派 Controller！RodAnimation 將停用");
            enabled = false;
            return;
        }
        // 若 Inspector 沒填長度，嘗試自動從 Clip 取得
        TryAutoLength(ref idleLen, idleState);
        TryAutoLength(ref castLen, castState);
        TryAutoLength(ref reelLen, reelState);
        Debug.Log($"[RodAnim:{gameObject.name}] idleLen={idleLen} castLen={castLen} reelLen={reelLen}");
    
    }
    
    public void PlayIdle()
    {
        if (IsBusy) return;
        anim.Play(idleState, 0, 0f);
    }

    public IEnumerator Play(Clip clip)
    {
        switch (clip)
        {
            case Clip.Cast:
                yield return PlayState(castState, castLen);
                break;
            case Clip.Reel:
                yield return PlayState(reelState, reelLen);
                break;
            case Clip.Idle:
            default:
                PlayIdle();
                break;
        }
    }

    /// <summary>通用：直接播放 Animator State 名並等待秒數。</summary>
    public IEnumerator PlayState(string stateName, float clipLength)
    {
        if (IsBusy) yield break;
        IsBusy = true;

        anim.CrossFade(stateName, 0f, 0, 0f); // 立即切該動作
        // 若 clipLength==0，採用 0.1s 預設以免無限迴圈
        yield return new WaitForSeconds(Mathf.Max(0.1f, clipLength));

        IsBusy = false;
    }
    
    void TryAutoLength(ref float lenField, string stateName)
    {
        if (lenField > 0f) return;                    // 已手填
        if (!anim.runtimeAnimatorController) return;  // 沒綁 Controller，直接離開

        foreach (var clip in anim.runtimeAnimatorController.animationClips)
            if (clip.name == stateName) { lenField = clip.length; break; }
    }
}