using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Game.Common;

namespace Game.Stamina
{
    public class StaminaController : MonoBehaviour
    {
        public static StaminaController Instance { get; private set; }

        [Header("參數設定")] public float Max = 100f;
        //public float DecayRate = 1f;
        public float PerCatchCost = 5f;

        public float Current { get; private set; }
        public StateID CurrentID { get; private set; }

        public BlinkAnimationModule BlinkModule { get; private set; }
        public StaminaVisualController VisualController { get; private set; }

        public enum StateID
        {
            Normal,
            Tired,
            WearyHigh,
            WearyLow,
            Exhausted,
            Overtired
        }

        readonly Dictionary<StateID, IState<StaminaController>> states = new();
        IState<StaminaController> currentState;

        public event Action<float> OnStaminaChanged;
        public event Action OnStaminaDepleted;

        void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            BlinkModule = GetComponentInChildren<BlinkAnimationModule>(true);
            VisualController = GetComponent<StaminaVisualController>();
        }

        void Start()
        {
            Current = Max;
            RegisterStates();
            SwitchTo(StateID.Normal);
            OnStaminaChanged?.Invoke(Current / Max);
        }

        void Update()
        {
            float dt = Time.deltaTime;
            //ChangeStamina(-DecayRate * dt);
            currentState.Tick(dt);
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

        public void OnCatchFish() => ChangeStamina(-PerCatchCost);

        void TrySwitchState()
        {
            float pct = Current / Max;
            StateID newID = pct switch
            {
                <= 0 => StateID.Overtired,
                <= 0.25f => StateID.Exhausted,
                <= 0.5f => StateID.WearyLow,
                <= 0.7f => StateID.WearyHigh,
                <= 0.8f => StateID.Tired,
                _ => StateID.Normal
            };
            if (newID != CurrentID) SwitchTo(newID);
        }

        void SwitchTo(StateID id)
        {
            currentState?.Exit();
            CurrentID = id;
            currentState = states[id];
            currentState.Enter(this);
        }

        void RegisterStates()
        {
            states[StateID.Normal] = new NormalState();
            states[StateID.Tired] = new TiredState();
            states[StateID.WearyHigh] = new WearyHighState();
            states[StateID.WearyLow] = new WearyLowState();
            states[StateID.Exhausted] = new ExhaustedState();
            states[StateID.Overtired] = new OvertiredState();
        }
    }
}
