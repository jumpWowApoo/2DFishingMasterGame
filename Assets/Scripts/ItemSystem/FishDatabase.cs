using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>集中管理所有魚種。</summary>
[CreateAssetMenu(menuName = "Fishing/Fish Database", order = 1)]
public class FishDatabase : ScriptableObject
{
    [SerializeField] List<FishData> fishTable = new();
    Dictionary<string, FishData> dict;   // 名稱 → FishData 快取

    void OnEnable() => dict = fishTable.ToDictionary(f => f.fishName);

    /// <summary>依名稱取得魚種。</summary>
    public FishData GetByName(string name) =>
        dict.TryGetValue(name, out var fd) ? fd : null;

    /// <summary>依權重隨機抽一種魚種。</summary>
    public FishData RandomPick()
    {
        float total = fishTable.Sum(f => f.weight);
        float r = Random.Range(0, total);
        foreach (var f in fishTable)
        {
            if (r < f.weight) return f;
            r -= f.weight;
        }
        return null;
    }

    public IReadOnlyList<FishData> All => fishTable;
}