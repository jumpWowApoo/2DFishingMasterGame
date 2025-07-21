using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[RequireComponent(typeof(FishingLine))]
[RequireComponent(typeof(BobberMotion))]
public class FishingController : MonoBehaviour
{
    public enum StateID
    {
        Idle,
        Casting,
        Fishing,
        FishBite,
        ReelIn,
        CaughtSuccess,
        CaughtFail,
        Baiting
    }

    [Header("UI / Refs")] public Button castButton;
    public Button reelButton;
    public Transform rodTip;
    public Transform targetPos;
    public GameObject bobberPrefab;
    public Transform BobberAttachPoint { get; private set; }

    [Header("Params")] public Vector2 waitRange = new(5, 10);
    public float biteAutoTime = 3f;
    public float successWindow = 2f;
    [SerializeField] float baitAnimLen = 1.0f;
    [SerializeField] RodAnimation rodAnim;

    readonly Dictionary<StateID, IFishingState> map = new();
    IFishingState current;
    public StateID CurrentID { get; private set; }
    public FishingLine Line { get; private set; }
    public GameObject CurrentBobber { get; private set; }
    public event Action<bool> OnResult;
    public FishingState FishingStateRef { get; private set; } // 新增

    void Start()
    {
        Line = GetComponent<FishingLine>();
        var mot = GetComponent<BobberMotion>();
        map[StateID.Idle] = new IdleState(this, castButton, reelButton, rodAnim);
        map[StateID.Casting] =
            new CastingState(this, rodTip, targetPos, bobberPrefab, mot, Line, rodAnim, castButton, reelButton);
        var fs = new FishingState(this, waitRange, reelButton, rodAnim);
        FishingStateRef = fs; // GUI 用
        map[StateID.Fishing] = fs; // 導演切換用
        map[StateID.FishBite] = new FishBiteState(this, biteAutoTime, successWindow, rodAnim);
        map[StateID.Baiting] = new BaitingState(this, rodAnim);
        map[StateID.ReelIn] =
            new ReelInState(this, Line, null, false, false, castButton, reelButton, rodAnim);

        SwitchTo(StateID.Idle);
    }

    void Update() => current?.Tick();

    public void SwitchTo(StateID id)
    {
        current?.OnExit();
        CurrentID = id;
        current = map[id];
        current.OnEnter();
    }

    public void SetBobber(GameObject bob)
    {
        CurrentBobber = bob;
        BobberAttachPoint = bob.GetComponentsInChildren<Transform>(true)
                                .FirstOrDefault(t => t.name == "LineAttachPoint")
                            ?? bob.transform;
    }

    // needBait: true=進掛餌動畫 ; false=直接回 Idle
    public void BeginReel(bool success, bool needBait)
    {
        current?.OnExit();
        current = new ReelInState(this, Line, CurrentBobber, success, needBait, castButton, reelButton, rodAnim);
        CurrentID = StateID.ReelIn;
        current.OnEnter();
    }

    public void EndReel(bool success, bool needBait)
    {
        OnResult?.Invoke(success);
        if (needBait)
        {
            current = new ResultState(this, success, castButton, reelButton, rodAnim);
            CurrentID = success ? StateID.CaughtSuccess : StateID.CaughtFail;
            current.OnEnter();
        }
        else
        {
            SwitchTo(StateID.Idle);
        }
    }
}