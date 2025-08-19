using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Inventory;


public class MissionMgr : MonoBehaviour
{
    public enum OrderMode
    {
        Sequential,
        Random
    }

    [Header("設定")] [SerializeField] MissionDatabase db;
    [SerializeField] OrderMode orderMode = OrderMode.Sequential;

    [Header("背包參照")] [SerializeField] InventoryMgr inventory; // 拖入單例或場景物件

    public MissionData Current { get; private set; }
    public event Action<MissionData> OnMissionChanged;
    public event Action OnMissionComplete;

    int seqIndex = 0;
    readonly System.Random rng = new();

    void Awake() => LoadNextMission();

    /* 提交任務 */
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
        StartCoroutine(DelaySwitch());
    }

    IEnumerator DelaySwitch()
    {
        yield return new WaitForSeconds(2f); // 顯示「更新中…」的時間
        LoadNextMission(); // 之後才廣播 OnMissionChanged
    }

    /* 取得下一任務 */
    void LoadNextMission()
    {
        if (db.all == null || db.all.Count == 0)
        {
            Debug.LogWarning("MissionDatabase 為空！");
            return;
        }

        Current = orderMode switch
        {
            OrderMode.Sequential => db.all[seqIndex++ % db.all.Count],
            OrderMode.Random => db.all[rng.Next(db.all.Count)],
            _ => db.all[0]
        };
        OnMissionChanged?.Invoke(Current);
    }
}