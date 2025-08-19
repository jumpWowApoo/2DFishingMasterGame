using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Inventory;
using Game.Common;

[DefaultExecutionOrder(-80)]                // ★ 讓它比多數 UI 更早初始化
public class MissionMgr : MonoBehaviour, IResettable
{
    public enum OrderMode { Sequential, Random }

    [Header("設定")] [SerializeField] MissionDatabase db;
    [SerializeField] OrderMode orderMode = OrderMode.Sequential;

    [Header("背包參照")] [SerializeField] InventoryMgr inventory;

    public MissionData Current { get; private set; }
    public event Action<MissionData> OnMissionChanged;
    public event Action OnMissionComplete;

    int seqIndex = 0;
    readonly System.Random rng = new();
    Coroutine switchingRoutine;

    void Awake()
    {
        LoadNextMission();  // 遊戲啟動就有任務
    }

    public void Submit(List<FishItem> delivered)
    {
        if (Current != null)
        {
            if (SessionRunLog.I != null)
                SessionRunLog.I.LogMission(Current, Current.rewardGold);

            if (Current.rewardGold > 0 && Game.Currency.Wallet.Instance != null)
                Game.Currency.Wallet.Instance.Add(Current.rewardGold);
        }

        OnMissionComplete?.Invoke();

        if (switchingRoutine != null) StopCoroutine(switchingRoutine);
        switchingRoutine = StartCoroutine(DelaySwitch());
    }

    IEnumerator DelaySwitch()
    {
        yield return new WaitForSeconds(2f);
        switchingRoutine = null;
        LoadNextMission();
    }

    public void ResetForNewRound(ResetLevel level)
    {
        if (switchingRoutine != null)
        {
            StopCoroutine(switchingRoutine);
            switchingRoutine = null;
        }
        if (level == ResetLevel.Hard) seqIndex = 0;
        LoadNextMission(); // 內部會觸發 OnMissionChanged
    }

    void LoadNextMission()
    {
        if (db == null || db.all == null || db.all.Count == 0)
        {
            Debug.LogWarning("MissionDatabase 為空！");
            Current = null;
            OnMissionChanged?.Invoke(Current);
            return;
        }

        Current = orderMode switch
        {
            OrderMode.Sequential => db.all[seqIndex++ % db.all.Count],
            OrderMode.Random     => db.all[rng.Next(db.all.Count)],
            _                    => db.all[0]
        };
        OnMissionChanged?.Invoke(Current);
    }

    // ★ 新增：給晚訂閱的 UI 叫用，馬上重播目前任務
    public void Rebroadcast()
    {
        if (Current != null) OnMissionChanged?.Invoke(Current);
    }
}
