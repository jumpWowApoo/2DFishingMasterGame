using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Animator))]
public class BobberAnimation : MonoBehaviour
{
    public enum Clip { Idle, Float, Sink }

    [Header("Bobber Animator State 名稱")]
    [SerializeField] string idleState  = "Bobber_Idle";
    [SerializeField] string floatState = "Bobber_Float";
    [SerializeField] string sinkState  = "Bobber_Sink";

    [Header("片段長度 (秒) 0=自動偵測")]
    [SerializeField] float idleLen  = 0f;
    [SerializeField] float floatLen = 0f;
    [SerializeField] float sinkLen  = 0.3f;

    [FormerlySerializedAs("forceAnimator")] [SerializeField] Animator targetAnimator;
    Animator ani;

    void Awake()
    {
        ani = targetAnimator ??
              GetComponent<Animator>() ??
              GetComponentInChildren<Animator>(true);

        Debug.Log($"[BobberAnimation] 找到 Animator & Controller：{ani.runtimeAnimatorController.name}", this);
        AutoLen(ref idleLen,  idleState);
        AutoLen(ref floatLen, floatState);
        AutoLen(ref sinkLen,  sinkState);
    }


    /* ───── 公開 API ───── */
    public void PlayIdle() => ani.Play(idleState, 0, 0f);

    public IEnumerator Play(Clip clip)
    {
        switch (clip)
        {
            case Clip.Float: yield return PlayState(floatState, floatLen); break;
            case Clip.Sink:  yield return PlayState(sinkState,  sinkLen);  break;
            default:         PlayIdle();                                   break;
        }
    }

    #region helpers
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