using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(FishingLine))]
[RequireComponent(typeof(BobberMotion))]
public class FishingController : MonoBehaviour
{
    public enum StateID { Idle, Casting, Baiting,ReelIn }

    [Header("UI / Refs")]
    [SerializeField] Button    castButton;
    [SerializeField] Transform rodTip;
    [SerializeField] Transform targetPos;
    [SerializeField] GameObject bobberPrefab;

    Dictionary<StateID, IFishingState> map = new();
    IFishingState current;
    public StateID CurrentID { get; private set; }
    void Awake()
    {
        var line   = GetComponent<FishingLine>();
        var motion = GetComponent<BobberMotion>();

        map[StateID.Idle]    = new IdleState(this, castButton);
        map[StateID.Casting] = new CastingState(this, rodTip, targetPos, bobberPrefab, motion, line);
        map[StateID.Baiting] = new BaitingState(this);

        SwitchTo(StateID.Idle);
    }

    void Update() => current?.Tick();

    public void SwitchTo(StateID id)
    {
        current?.OnExit();
        CurrentID = id;
        current   = map[id];
        current.OnEnter();
    }
    public void BeginReel(FishingLine line, GameObject bobber, bool success)
    {
        current?.OnExit();                           // 退出現狀態
        current   = new ReelInState(this, line, bobber, success);
        CurrentID = StateID.ReelIn;
        current.OnEnter();
    }
}