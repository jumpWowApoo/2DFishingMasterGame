using UnityEngine;

namespace Game.Consumables
{
    /// <summary>
    /// 遊戲中持有的「消耗品道具實體」。
    /// data 是靜態資料 (ScriptableObject)，count 是玩家目前擁有的數量。
    /// </summary>
    [System.Serializable]
    public class ConsumableItem
    {
        public ConsumableData data; // 參照 ScriptableObject
        public int count = 1;       // 目前數量

        public ConsumableItem(ConsumableData data, int count = 1)
        {
            this.data = data;
            this.count = count;
        }

        /// <summary>
        /// 使用道具 → 呼叫其所有效果。
        /// 成功使用時會消耗數量。
        /// </summary>
        public bool Use(ConsumableContext ctx)
        {
            if (data == null || count <= 0) return false;

            // 執行所有效果
            data.Use(ctx);

            // 消耗數量
            count--;

            Debug.Log($"使用 {data.itemName}，剩餘 {count} 個");
            return true;
        }

        /// <summary>
        /// 是否還能使用（有數量且有資料）。
        /// </summary>
        public bool CanUse => data != null && count > 0;
    }
}