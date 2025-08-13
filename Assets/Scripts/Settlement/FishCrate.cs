using System.Collections.Generic;
using UnityEngine;

public class FishCrate : MonoBehaviour
{
    public static FishCrate I { get; private set; }

    readonly Dictionary<FishData, int> counts = new();

    void Awake()
    {
        if (I && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    // 加入（釣到魚成功時）
    public void Add(FishItem item, int qty = 1) => Add(item?.data, qty);

    public void Add(FishData data, int qty = 1)
    {
        if (!data || qty <= 0) return;
        if (!counts.ContainsKey(data)) counts[data] = 0;
        counts[data] += qty;
    }

    // 扣除（任務交付 / 丟棄 / 之後 NPC 偷）
    public int Remove(FishData data, int qty = 1)
    {
        if (!data || qty <= 0 || !counts.ContainsKey(data)) return 0;
        int take = Mathf.Min(qty, counts[data]);
        counts[data] -= take;
        if (counts[data] <= 0) counts.Remove(data);
        return take;
    }

    public Dictionary<FishData, int> GetSnapshot() => new(counts);

    public int ComputeTotalPrice()
    {
        int sum = 0;
        foreach (var kv in counts) sum += kv.Key.sellPrice * kv.Value;
        return sum;
    }

    public void Clear() => counts.Clear();

    public bool IsEmpty => counts.Count == 0;
}