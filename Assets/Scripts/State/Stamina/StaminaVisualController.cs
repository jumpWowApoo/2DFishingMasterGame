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
            // Blink 模組自動抓
            if (blinkModule == null)
                blinkModule = GetComponentInChildren<BlinkAnimationModule>(true);

            // Volume 自動抓
            if (volume == null)
                volume = GetComponentInChildren<Volume>(true);
            if (volume == null && Camera.main)
                volume = Camera.main.GetComponentInChildren<Volume>(true);
            if (volume == null && Camera.main) {
                Transform p = Camera.main.transform.parent;
                while (p && volume == null) {
                    volume = p.GetComponent<Volume>() ?? p.GetComponentInChildren<Volume>(true);
                    p = p.parent;
                }
            }
            if (volume == null) {
                foreach (var v in FindObjectsOfType<Volume>(true)) {
                    if (v.isGlobal) { volume = v; break; }
                }
            }

            if (volume && volume.profile) {
                volume.profile.TryGet(out vignette);
                volume.profile.TryGet(out colorAdj);
            } else {
                Debug.LogWarning("[StaminaVisualController] 找不到可用的 Volume，視覺效果將停用。");
            }

            // 先嘗試抓 Controller
            ctx = GetComponentInParent<StaminaController>();
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
                blinkModule.SetBlink(0f, 0f);
                vignette.intensity.value = 0f;
                colorAdj.contrast.value  = 0f;
                colorAdj.saturation.value= 0f;
            }
            else if (pct > 0.70f) {
                blinkModule.SetBlink(10f, 1f);
                vignette.intensity.value = 0f;
                colorAdj.contrast.value  = 0f;
                colorAdj.saturation.value= 0f;
            }
            else if (pct > 0.50f) {
                blinkModule.SetBlink(5f, 1f);
                vignette.intensity.value  = 0.15f;
                vignette.smoothness.value = 1f;
                colorAdj.contrast.value   = 10f;
                colorAdj.saturation.value = -15f;
            }
            else if (pct > 0.25f) {
                blinkModule.SetBlink(5f, 1.2f);
                vignette.intensity.value  = 0.25f;
                vignette.smoothness.value = 0.7f;
                colorAdj.contrast.value   = 10f;
                colorAdj.saturation.value = -15f;
            }
            else {
                blinkModule.SetBlink(5f, 1.5f);
                vignette.intensity.value  = 0.40f;
                vignette.smoothness.value = 0.8f;
                colorAdj.contrast.value   = 20f;
                colorAdj.saturation.value = -30f;
            }
        }
    }
}
