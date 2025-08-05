// StaminaVisualController.cs
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Game.Stamina {
    /// <summary>
    /// 依體力百分比調整 Post‑Process 與眨眼頻率。
    /// 自動尋找 Volume；若 Controller 尚未建立則延遲訂閱。
    /// </summary>
    public class StaminaVisualController : MonoBehaviour {
        [Header("眨眼模組")] [SerializeField] BlinkAnimationModule blinkModule;
        [Header("Post-Process Volume")] [SerializeField] Volume volume;

        Vignette         vignette;
        ColorAdjustments colorAdj;
        StaminaController ctx;

        void Awake() {
            if (!blinkModule) blinkModule = GetComponentInChildren<BlinkAnimationModule>(true);

            volume = volume ? volume : FindVolume();          // 自動尋 Volume
            if (volume && volume.profile) {
                volume.profile.TryGet(out vignette);
                volume.profile.TryGet(out colorAdj);
            }

            ctx = GetComponentInParent<StaminaController>();  // 可能為 null，待 OnEnable 再檢
        }
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

        void OnEnable() {
            if (ctx == null) ctx = StaminaController.Instance;
            if (ctx != null)
                ctx.OnStaminaChanged += HandleVisual;
            else
                StartCoroutine(WaitForController());
        }
        void OnDisable() {
            if (ctx != null)
                ctx.OnStaminaChanged -= HandleVisual;
        }

        IEnumerator WaitForController() {
            while (ctx == null) {
                ctx = StaminaController.Instance;
                if (ctx != null) {
                    ctx.OnStaminaChanged += HandleVisual;
                    yield break;
                }
                yield return null; // 下一幀再試
            }
        }

        void HandleVisual(float pct) {
            if (!blinkModule || vignette == null || colorAdj == null) return;

            if (pct > 0.80f) {
                vignette.intensity.value = 0f;
                colorAdj.contrast.value  = 0f;
                colorAdj.saturation.value= 0f;
            }
            else if (pct > 0.70f) {
                vignette.intensity.value = 0f;
                colorAdj.contrast.value  = 0f;
                colorAdj.saturation.value= 0f;
            }
            else if (pct > 0.50f) {
                vignette.intensity.value  = 0.15f;
                vignette.smoothness.value = 1f;
                colorAdj.contrast.value   = 10f;
                colorAdj.saturation.value = -15f;
            }
            else if (pct > 0.25f) {
                vignette.intensity.value  = 0.25f;
                vignette.smoothness.value = 0.7f;
                colorAdj.contrast.value   = 10f;
                colorAdj.saturation.value = -15f;
            }
            else {
                vignette.intensity.value  = 0.40f;
                vignette.smoothness.value = 0.8f;
                colorAdj.contrast.value   = 20f;
                colorAdj.saturation.value = -30f;
            }
        }
    }
}
