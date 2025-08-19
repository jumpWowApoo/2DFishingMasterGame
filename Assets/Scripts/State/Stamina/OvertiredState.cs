using Game.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Stamina {
    public class OvertiredState : IState<StaminaController> {
        public void Enter(StaminaController ctx) {
            Debug.Log("跳到結束場景");
            //SettlementFlow.OpenSettlement();
            SceneManager.LoadScene("GameOverScene");
        }
        public void Tick(float dt) {}
        public void Exit() {}
    }
}