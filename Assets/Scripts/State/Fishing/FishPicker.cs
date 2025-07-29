// Assets/Scripts/Gameplay/Fishing/FishPicker.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class FishPicker
{
    /// <summary>回傳依權重隨機抽出的 FishData。</summary>
    public static FishData PickRandomFish(List<FishData> pool)
    {
        float total = pool.Sum(f => f.weight);
        float r = Random.value * total;
        foreach (var f in pool)
        {
            if (r < f.weight) return f;
            r -= f.weight;
        }
        return pool[^1];   // 保底
    }
}