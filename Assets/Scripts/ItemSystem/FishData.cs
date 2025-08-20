using UnityEngine;

/// <summary>單條魚的靜態資料。</summary>
[CreateAssetMenu(menuName = "Fishing/Fish Data")]
public class FishData : ScriptableObject
{
    public string fishId; //編號
    public string fishName; // 名稱
    public int sellPrice = 10;
    public Sprite icon; // 小圖 (背包用)
    public Sprite discardSprite;
    public Sprite bigImage; // 大圖 (資訊窗用)
    [Range(0, 1)] public float weight = 0.33f; // 抽中權重
    [TextArea] public string description;

    [Header("拖曳時生成的視覺 Prefab")] public GameObject dragItemPrefab;
}