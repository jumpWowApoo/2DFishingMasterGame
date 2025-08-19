using System;
using System.Collections.Generic;
using UnityEngine;
using Game.Common;

namespace Game.Stamina
{
    public class StaminaController : MonoBehaviour, IResettable
    {
        public static StaminaController Instance { get; private set; }

        [Header("參數設定")] public float Max = 100f;
        public float PerCatchCost = 5f;

        public float Current { get; private set; }
        public StateID CurrentID { get; private set; }

        public BlinkAnimationModule BlinkModule { get; private set; }
        public StaminaVisualModule VisualModule { get; private set; }

        public enum StateID { Normal, Tired, WearyHigh, WearyLow, Exhausted, Overtired }

        readonly Dictionary<StateID, IState<StaminaController>> states = new();
        IState<StaminaController> currentState;

        public event Action<float> OnStaminaChanged;
        public event Action OnStaminaDepleted;

        void Awake()
        {
            if (Instance && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            VisualModule = GetComponent<StaminaVisualModule>();
            BlinkModule  = GetComponentInChildren<BlinkAnimationModule>(true);
            Current = Max;          // 先把數值就位
            RegisterStates();       // 先把狀態表建立好
            SwitchTo(StateID.Normal);          // 再切到 Normal（此時字典已經有鍵）
            OnStaminaChanged?.Invoke(Current / Max);
        }

        void Start()
        {
        }

        void Update()
        {
            float dt = Time.deltaTime;
            currentState?.Tick(dt);
            TrySwitchState();
        }

        public void ChangeStamina(float delta)
        {
            float old = Current;
            Current = Mathf.Clamp(Current + delta, 0f, Max);
            if (!Mathf.Approximately(old, Current))
            {
                OnStaminaChanged?.Invoke(Current / Max);
                if (Current <= 0f) OnStaminaDepleted?.Invoke();
            }
        }

        public void OnCatchFish() => ChangeStamina(-PerCatchCost / 100f);

        void TrySwitchState()
        {
            float pct = (Max > 0f) ? Current / Max : 0f;
            StateID newID = pct switch
            {
                <= 0     => StateID.Overtired,
                <= 0.25f => StateID.Exhausted,
                <= 0.5f  => StateID.WearyLow,
                <= 0.7f  => StateID.WearyHigh,
                <= 0.9f  => StateID.Tired,
                _        => StateID.Normal
            };
            if (newID != CurrentID) SwitchTo(newID);
        }

        void SwitchTo(StateID id)
        {
            currentState?.Exit();

            if (!states.TryGetValue(id, out var next))
            {
                // 若還沒註冊就補一次，然後再試
                if (states.Count == 0) RegisterStates();
                if (!states.TryGetValue(id, out next))
                    return; // 還是沒有就先跳過，避免例外
            }

            CurrentID = id;
            currentState = next;
            currentState.Enter(this);
        }


        void RegisterStates()
        {
            states[StateID.Normal]     = new NormalState();
            states[StateID.Tired]      = new TiredState();
            states[StateID.WearyHigh]  = new WearyHighState();
            states[StateID.WearyLow]   = new WearyLowState();
            states[StateID.Exhausted]  = new ExhaustedState();
            states[StateID.Overtired]  = new OvertiredState();
        }

        // 受控初始化：回滿 + 回 Normal + 同步 UI
        public void ResetForNewRound(ResetLevel level)
        {
            Current = Max;
            SwitchTo(StateID.Normal);
            OnStaminaChanged?.Invoke(Current / Max);
        }
    }
}
