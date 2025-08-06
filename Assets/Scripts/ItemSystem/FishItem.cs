using Game.Inventory;
using UnityEngine;

public class FishItem : InventoryItemBase
{
    public readonly FishData data;

    public string id => data.fishId;
    public override Sprite Icon => data.icon;
    public Sprite BigImage => data.bigImage;
    public override string Name => data.fishName;

    public FishItem(FishData data) => this.data = data;
}