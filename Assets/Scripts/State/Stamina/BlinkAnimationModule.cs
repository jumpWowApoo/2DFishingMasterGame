using System.Collections;
using UnityEngine;

namespace Game.Stamina
{
    /// <summary>
    /// 每隔 interval 秒觸發 BlinkTrigger，一進入新階段先眨一次。
    /// </summary>
    public class BlinkAnimationModule : MonoBehaviour
    {
        [SerializeField] Animator animator;
        [SerializeField] string blinkTrigger = "BlinkTrigger";

        public float CurrentInterval { get; private set; }
        public float CurrentSpeed { get; private set; }

        Coroutine blinkRoutine;

        void Awake()
        {
            if (!animator) animator = GetComponentInChildren<Animator>(true);
            if (!animator)
                Debug.LogWarning("[BlinkAnimationModule] 無 Animator，眨眼停用。");
        }

        /// <param name="interval">眨眼間隔 (秒)。<=0 停用</param>
        /// <param name="speed">Animator.speed 倍速 (1=正常)</param>
        public void SetBlink(float interval, float speed)
        {
            if (!animator) return;

            /* 相同設定就別重設，避免反覆觸發 */
            if (Mathf.Approximately(interval, CurrentInterval) &&
                Mathf.Approximately(speed, CurrentSpeed))
                return;

            CurrentInterval = interval;
            CurrentSpeed = speed;
            animator.speed = speed;

            /* 先停舊協程 */
            if (blinkRoutine != null)
            {
                StopCoroutine(blinkRoutine);
                blinkRoutine = null;
            }

            /* interval <= 0 → 完全停用眨眼 */
            if (interval <= 0f) return;

            /* 立即眨一次（Reset→Set 保證旗標清乾淨） */
            animator.ResetTrigger(blinkTrigger);
            animator.SetTrigger(blinkTrigger);

            /* 再啟新的固定間隔協程 */
            blinkRoutine = StartCoroutine(BlinkLoop(interval));
        }

        IEnumerator BlinkLoop(float interval)
        {
            while (true)
            {
                animator.ResetTrigger(blinkTrigger); // 清舊旗標
                animator.SetTrigger(blinkTrigger); // 送新旗標
                yield return new WaitForSeconds(interval);
            }
        }

        void OnDisable()
        {
            if (blinkRoutine != null) StopCoroutine(blinkRoutine);
            blinkRoutine = null;
            if (animator) animator.speed = 1f;
        }
    }
}