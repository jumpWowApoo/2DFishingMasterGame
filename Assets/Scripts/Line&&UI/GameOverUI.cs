using UnityEngine;
using UnityEngine.SceneManagement;
using Game.Inventory;

namespace Game.Stamina
{
    public class GameOverUI : MonoBehaviour
    {
        /// <summary>
        /// 推薦：先進「結算」，由結算面板計價入錢包並清空魚箱；
        /// 結算完成後自動回到當前遊戲場景（不重載，不清背包）。
        /// </summary>
        public void SettleThenReturn()
        {
            // 若已經在結算就不重複開
            SettlementFlow.OpenSettlement();
        }

        /// <summary>
        /// 後門：強制重開關卡（清背包＋清魚箱＋重載場景）。
        /// 只有當你真的要完全重置一局時才使用。
        /// </summary>
        public void HardRestart()
        {
            var ctx = StaminaController.Instance;
            if (ctx != null)
            {
                // 將體力補到滿（或你要的初始值）
                ctx.ChangeStamina(ctx.Max - ctx.Current);
            }

            // 清背包（你的原邏輯）
            if (InventoryMgr.Instance != null)
                InventoryMgr.Instance.Clear();

            // ★ 同步清空魚箱，避免重載後舊魚被結算到
            if (FishCrate.I != null)
                FishCrate.I.Clear();

            // 重載場景
            SceneManager.LoadScene("S1");
        }
    }
}