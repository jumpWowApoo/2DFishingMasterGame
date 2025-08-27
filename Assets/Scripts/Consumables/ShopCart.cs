using System.Collections.Generic;

namespace Game.Consumables.Shop
{
    public class ShopCart
    {
        private readonly Dictionary<string, OrderLine> _map = new();
        public IReadOnlyDictionary<string, OrderLine> Lines => _map;

        public void SetQuantity(ConsumableData data, int unitPrice, int qty)
        {
            if (data == null) return;
            if (qty <= 0) { _map.Remove(data.itemId); return; }

            if (_map.TryGetValue(data.itemId, out var line))
            {
                line.quantity = qty;        // 覆寫
                line.unitPrice = unitPrice;
            }
            else
            {
                _map[data.itemId] = new OrderLine { data = data, unitPrice = unitPrice, quantity = qty };
            }
        }

        public bool TryGet(string itemId, out OrderLine line) => _map.TryGetValue(itemId, out line);
        public void Clear() => _map.Clear();
        public bool IsEmpty => _map.Count == 0;

        public int Total()
        {
            int t = 0;
            foreach (var kv in _map) t += kv.Value.Subtotal;
            return t;
        }
    }

    public class OrderLine
    {
        public ConsumableData data;
        public int unitPrice;
        public int quantity;
        public int Subtotal => unitPrice * quantity;
    }
}