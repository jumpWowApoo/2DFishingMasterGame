using System.Collections.Generic;
using UnityEngine;
using Game.Common;

public class FishCrate : MonoBehaviour, IResettable
{
    public static FishCrate I { get; private set; }

    // 事件：UI 或其他系統可訂閱
    public System.Action OnChanged;
    public System.Action<FishData, int> OnAdded;
    public System.Action<FishData, int> OnRemoved;

    readonly Dictionary<FishData, int> counts = new();

    void Awake()
    {
        if (I && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    // 便利多載
    public void Add(FishItem item, int qty = 1)
    {
        if (item == null || item.data == null || qty <= 0) return;
        Add(item.data, qty);
    }

    public void Add(FishData data, int qty = 1)
    {
        if (!data || qty <= 0) return;

        if (!counts.ContainsKey(data)) counts[data] = 0;
        counts[data] += qty;
        OnAdded?.Invoke(data, qty);
        OnChanged?.Invoke();
    }

    /// <summary>
    /// 從魚箱取出指定數量；回傳實際取出的數量。
    /// </summary>
    public int Remove(FishData data, int qty = 1)
    {
        if (!data || qty <= 0 || !counts.ContainsKey(data)) return 0;

        int take = Mathf.Min(qty, counts[data]);
        if (take <= 0) return 0;

        counts[data] -= take;
        if (counts[data] <= 0) counts.Remove(data);
        OnRemoved?.Invoke(data, take);
        OnChanged?.Invoke();
        return take;
    }

    public Dictionary<FishData, int> GetSnapshot() => new(counts);

    public int ComputeTotalPrice()
    {
        int sum = 0;
        foreach (var kv in counts) sum += kv.Key.sellPrice * kv.Value;
        return sum;
    }

    public void Clear()
    {
        if (counts.Count == 0) return;
        counts.Clear();
        OnChanged?.Invoke();
    }

    public bool IsEmpty => counts.Count == 0;

    public void ResetForNewRound(ResetLevel level)
    {
        Clear(); // 新的一輪一律清空
    }
}
