using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;



namespace Game.Stamina {
    /// <summary>
    /// 只提供「設定後製效果」的工具。
    /// 誰要用就在 State.Enter() 呼叫，Module 本身不訂閱體力事件。
    /// </summary>
    public class StaminaVisualModule : MonoBehaviour {
        [SerializeField] Volume volume;
        [SerializeField] float  fadeTime = 1f;   // 平滑秒數

        Vignette         vignette;
        ColorAdjustments colorAdj;
        Coroutine        oscRoutine;
        
        float targetIntensity, targetSmooth;
        float targetContrast,  targetSaturation;
        bool   oscillate;
        float  oscMin, oscMax, oscSpeed;

        void Awake() {
            volume = volume ? volume : FindVolume();          // 自動尋 Volume
            if (volume && volume.profile) {
                volume.profile.TryGet(out vignette);
                volume.profile.TryGet(out colorAdj);
            }
        }
        
        void Update () {
            if (!vignette) return;

            /* 先平滑逼近 targetIntensity */
            float t = Time.deltaTime / fadeTime;
            float baseVal = Mathf.Lerp(vignette.intensity.value, targetIntensity,  t);

            /* 再疊加震盪（若有開啟）*/
            if (oscillate) {
                float osc = Mathf.Lerp(oscMin, oscMax, Mathf.PingPong(Time.time * oscSpeed, 1f));
                vignette.intensity.value = osc;          // 直接用振幅值
            } else {
                vignette.intensity.value = baseVal;      // 僅平滑
            }

            /* 其餘項目仍平滑到目標 */
            vignette.smoothness.value = Mathf.Lerp(vignette.smoothness.value, targetSmooth, t);
            colorAdj.contrast.value   = Mathf.Lerp(colorAdj.contrast.value,   targetContrast,  t);
            colorAdj.saturation.value = Mathf.Lerp(colorAdj.saturation.value, targetSaturation,t);
        }


        /* ---------- 各階段 API ---------- */
        public void SetNormal() {                        // ≥90%
            if (!vignette) return;
            oscillate        = false;
            targetIntensity  = 0f;
            targetSmooth     = 0f;
            targetContrast   = 0f;
            targetSaturation = 0f;
        }
        public void SetTired() {                         // 70~90%
            oscillate        = false;
            targetIntensity  = 0.20f;
            targetSmooth     = 1f;
            targetContrast   = 0f;
            targetSaturation = 0f;
        }
        public void SetWearyHigh() {                     // 50~70%
            oscillate        = true;
            oscMin = 0.25f; oscMax = 0.30f; oscSpeed = 0.5f;
            targetSmooth     = 0.7f;
            targetContrast   = 10f;
            targetSaturation = -15f;
        }
        public void SetWearyLow() {                      // 25~50%
            oscillate        = true;
            oscMin = 0.40f; oscMax = 0.50f; oscSpeed = 0.5f;
            targetSmooth     = 0.8f;
            targetContrast   = 20f;
            targetSaturation = -30f;
        }
        /* ---------- 輔助 ---------- */
        Volume FindVolume() {
            if (volume) return volume;
            if (Camera.main) {
                var v = Camera.main.GetComponentInChildren<Volume>(true);
                if (v) return v;
                // 往父鏈追
                var p = Camera.main.transform.parent;
                while (p) {
                    v = p.GetComponent<Volume>() ?? p.GetComponentInChildren<Volume>(true);
                    if (v) return v;
                    p = p.parent;
                }
            }
            foreach (var v in FindObjectsOfType<Volume>(true))
                if (v.isGlobal) return v;
            Debug.LogWarning("[StaminaVisualController] Volume 未找到。");
            return null;
        }
    }
}
