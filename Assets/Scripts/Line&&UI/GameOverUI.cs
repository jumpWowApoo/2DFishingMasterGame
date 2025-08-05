using UnityEngine;
using UnityEngine.SceneManagement;
using Game.Inventory;

namespace Game.Stamina {
    public class GameOverUI : MonoBehaviour {
        public void RestartGame() {
            var ctx = StaminaController.Instance;
            ctx.ChangeStamina(ctx.Max - ctx.Current);
            InventoryMgr.Instance.Clear();
            SceneManager.LoadScene("S1");
        }
    }
}