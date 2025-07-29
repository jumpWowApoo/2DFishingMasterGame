using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Game.UI;
using Game.Common;

[RequireComponent(typeof(FishingLine))]
[RequireComponent(typeof(BobberMotion))]
public class FishingController : MonoBehaviour
{
    /* ---------- 狀態列舉 ---------- */
    public enum StateID
    {
        Idle,
        Casting,
        Fishing,
        FishBite,
        ReelIn,
        Result,
        Baiting
    }

    /* ---------- Inspector 參照 ---------- */
    [Header("UI / Refs")] public Button castButton;
    public Button reelButton;
    public Transform rodTip;
    public Transform targetPos;
    public GameObject bobberPrefab;

    [Header("魚資料庫與 UI")] [SerializeField] FishDatabase db; // ★ 新增
    [SerializeField] FishInfoPanel infoPanel;
    [SerializeField] UIHub uiHub;

    /* ---------- 釣魚參數 ---------- */
    [Header("Params")] public Vector2 waitRange = new(5, 10);
    public float biteAutoTime = 3f;
    public float successWindow = 2f;
    [SerializeField] float baitAnimLen = 1.0f;
    [SerializeField] RodAnimation rodAnim;

    /* ---------- 內部欄位 ---------- */
    readonly Dictionary<StateID, IFishingState> map = new();
    IFishingState current;

    public StateID CurrentID { get; private set; }
    public FishingLine Line { get; private set; }
    public GameObject CurrentBobber { get; private set; }
    public Transform BobberAttachPoint { get; private set; }

    public FishingState FishingStateRef { get; private set; } // 提供 GUI 觀察
    public FishItem CurrentFishItem { get; private set; } // ★ 這次釣到的魚

    public event Action<bool> OnResult;

    /* ---------- 初始 ---------- */
    void Start()
    {
        Line = GetComponent<FishingLine>();
        var motion = GetComponent<BobberMotion>();
        uiHub.CloseAll();

        map[StateID.Idle] = new IdleState(this, castButton, reelButton, rodAnim);
        map[StateID.Casting] = new CastingState(this, rodTip, targetPos, bobberPrefab,
            motion, Line, rodAnim,
            castButton, reelButton);

        FishingStateRef = new FishingState(this, waitRange, reelButton, rodAnim);
        map[StateID.Fishing] = FishingStateRef;

        map[StateID.FishBite] = new FishBiteState(this, biteAutoTime, reelButton,successWindow, rodAnim);
        map[StateID.Baiting] = new BaitingState(this, rodAnim, castButton, reelButton);


        SwitchTo(StateID.Idle);
    }

    void Update() => current?.Tick();

    public void SwitchTo(StateID id)
    {
        current?.OnExit();
        if (id == StateID.Fishing)
        {
            FishData fd = db.RandomPick();
            CurrentFishItem = new FishItem(fd);
        }

        current = id switch
        {
            StateID.Idle => map[StateID.Idle],
            StateID.Casting => map[StateID.Casting],
            StateID.Fishing => map[StateID.Fishing],
            StateID.FishBite => map[StateID.FishBite],
            StateID.Baiting => map[StateID.Baiting],
            StateID.ReelIn => CreateReelInState(false, false),
            _ => null
        };

        CurrentID = id;
        current?.OnEnter();
    }

    public void SetBobber(GameObject bob)
    {
        CurrentBobber = bob;
        BobberAttachPoint = bob.GetComponentsInChildren<Transform>(true)
                                .FirstOrDefault(t => t.name == "LineAttachPoint")
                            ?? bob.transform;
    }

    // 初始化 Reeling
    public void BeginReel(bool success, bool needBait)
    {
        current?.OnExit();
        current = CreateReelInState(success, needBait);
        CurrentID = StateID.ReelIn;
        current.OnEnter();
    }

    // 收線動畫結束後呼叫
    public void EndReel(bool success, bool needBait)
    {
        OnResult?.Invoke(success);
        if (needBait)
        {
            current?.OnExit();
            current = new ResultState(this, success, infoPanel, uiHub);
            CurrentID = StateID.Result;
            current.OnEnter();
            return;
        }

        SwitchTo(StateID.Idle);
    }

    /* ---------- 工具：動態 new ReelInState ---------- */
    IFishingState CreateReelInState(bool success, bool needBait) =>
        new ReelInState(this, Line, CurrentBobber, success, needBait,
            castButton, reelButton, rodAnim);
}