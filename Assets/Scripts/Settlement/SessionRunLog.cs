using System.Collections.Generic;
using UnityEngine;

public class SessionRunLog : MonoBehaviour
{
    public static SessionRunLog I { get; private set; }

    [System.Serializable]
    public class MissionAgg
    {
        public string missionId;
        public string title;
        public int times;       // 完成次數
        public int rewardSum;   // 累計獎勵（目前可為 0）
    }

    // missionId -> 聚合
    readonly Dictionary<string, MissionAgg> _agg = new();

    void Awake()
    {
        if (I && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LogMission(MissionData data, int reward = 0)
    {
        if (!data) return;
        if (!_agg.TryGetValue(data.missionId, out var m))
        {
            m = new MissionAgg { missionId = data.missionId, title = data.title, times = 0, rewardSum = 0 };
            _agg[data.missionId] = m;
        }
        m.times++;
        m.rewardSum += reward;
    }

    public List<MissionAgg> GetAggregatedSnapshot() => new List<MissionAgg>(_agg.Values);

    public int ComputeMissionTotal()
    {
        int sum = 0;
        foreach (var m in _agg.Values) sum += m.rewardSum;
        return sum;
    }

    public (int kinds, int times) GetCounts()
    {
        int kinds = _agg.Count, times = 0;
        foreach (var m in _agg.Values) times += m.times;
        return (kinds, times);
    }

    public void ClearAll() => _agg.Clear();
}