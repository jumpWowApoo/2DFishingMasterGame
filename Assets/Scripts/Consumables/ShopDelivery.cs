using UnityEngine;

namespace Game.Consumables.Shop
{
    public class ShopDelivery : MonoBehaviour
    {
        // 把購物車所有品項逐件塞進 bag，回傳成功件數
        public int DeliverAll(ShopCart cart, ConsumableBag bag)
        {
            int delivered = 0;
            foreach (var kv in cart.Lines)
            {
                var line = kv.Value;
                for (int i = 0; i < line.quantity; i++)
                {
                    if (bag.TryAdd(line.data)) delivered++;
                    else
                    {
                        Debug.LogWarning($"[Shop] 背包已滿：{line.data.itemName} 有部分未送達");
                        break;
                    }
                }
            }
            return delivered;
        }
    }
}
