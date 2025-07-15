using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("UI / Refs")] 
    public Button castButton;
    public Button reelButton;
    public Transform rodTip;
    public Transform targetPos;
    public GameObject bobberPrefab;
    
    [Header("Params")] 
    public Vector2 waitRange = new(5, 10);
    public float biteAutoTime = 3f;
    public float successWindow = 2f;
    [SerializeField] Animator baitAnimator;
    [SerializeField] float baitAnimLen = 1.0f;

    readonly Dictionary<StateID, IFishingState> map = new();
    IFishingState current;
    public StateID CurrentID { get; private set; }
    public FishingLine Line { get; private set; }
    public GameObject CurrentBobber { get; private set; }
    public event Action<bool> OnResult;

    void Start()
    {
        Line = GetComponent<FishingLine>();
        var mot = GetComponent<BobberMotion>();
        map[StateID.Idle] = new IdleState(this, castButton, reelButton);
        map[StateID.Casting] =
            new CastingState(this, rodTip, targetPos, bobberPrefab, mot, Line, castButton, reelButton);
        map[StateID.Fishing] = new FishingState(this, waitRange,reelButton);
        map[StateID.FishBite] = new FishBiteState(this, biteAutoTime, successWindow);
        map[StateID.Baiting] = new BaitingState(this);
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

    public void SetBobber(GameObject bob) => CurrentBobber = bob;

    // needBait: true=進掛餌動畫 ; false=直接回 Idle
    public void BeginReel(bool success, bool needBait)
    {
        current?.OnExit();
        current = new ReelInState(this, Line, CurrentBobber, success, needBait, castButton, reelButton);
        CurrentID = StateID.ReelIn;
        current.OnEnter();
    }

    public void EndReel(bool success, bool needBait)
    {
        OnResult?.Invoke(success);
        if (needBait)
        {
            current = new ResultState(this, success, castButton, reelButton);
            CurrentID = success ? StateID.CaughtSuccess : StateID.CaughtFail;
            current.OnEnter();
        }
        else
        {
            SwitchTo(StateID.Idle);
        }
    }
}