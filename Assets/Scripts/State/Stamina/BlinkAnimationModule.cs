using System.Collections;
using UnityEngine;

namespace Game.Stamina {
    /// <summary>
    /// 控制眨眼動畫：直接播放指定 State，並以 Animator.speed 調整速度。
    /// 如果未手動綁定 Animator，將自動在子物件搜尋。
    /// </summary>
    public class BlinkAnimationModule : MonoBehaviour {
        [SerializeField] Animator animator;
        [SerializeField] string   blinkStateName = "Blink"; // Animator 中的狀態名稱

        Coroutine blinkRoutine;

        void Awake() {
            if (animator == null)
                animator = GetComponentInChildren<Animator>(true);
            if (animator == null)
                Debug.LogWarning("[BlinkAnimationModule] 找不到 Animator，眨眼功能將停用。");
        }

        /// <param name="interval">眨眼間隔（秒），<=0 代表停用</param>
        /// <param name="speed">播放速度倍數 (1 = 正常)</param>
        public void SetBlink(float interval, float speed) {
            if (animator == null) return;

            animator.speed = speed;

            if (blinkRoutine != null) {
                StopCoroutine(blinkRoutine);
                blinkRoutine = null;
            }
            if (interval > 0f) blinkRoutine = StartCoroutine(BlinkLoop(interval));
        }

        IEnumerator BlinkLoop(float interval) {
            while (true) {
                yield return new WaitForSeconds(interval);
                animator.Play(blinkStateName, 0, 0f);
            }
        }

        void OnDisable() {
            if (animator) animator.speed = 1f;
            if (blinkRoutine != null) StopCoroutine(blinkRoutine);
        }
    }
}