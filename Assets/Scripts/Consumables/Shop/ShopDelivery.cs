using UnityEngine;

namespace Game.Consumables.Shop
{
    public class ShopDelivery : MonoBehaviour
    {
        /// <summary>把購物車所有品項逐件塞進 bag，回傳成功件數。</summary>
        public int DeliverAll(ShopCart cart, ConsumableBag bag)
        {
            if (cart == null || bag == null) return 0;

            int delivered = 0;
            foreach (var kv in cart.Lines)
            {
                var line = kv.Value;
                for (int i = 0; i < line.quantity; i++)
                {
                    bool ok = bag.TryAdd(line.data);
                    if (ok) delivered++;
                    else
                    {
                        Debug.LogWarning($"[ShopDelivery] 背包已滿，{line.data?.name} 無法放入。已送達 {delivered} 件。");
                        return delivered; // 空間不足直接結束
                    }
                }
            }
            Debug.Log($"[ShopDelivery] 已送入背包 {delivered} 件。");
            return delivered;
        }
    }
}