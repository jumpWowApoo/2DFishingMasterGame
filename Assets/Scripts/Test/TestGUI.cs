using System;
using System.Collections;
using System.Collections.Generic;
using Game.Stamina;
using UnityEngine;

public class TestGUI : MonoBehaviour
{
    [Header("參考")]
    [SerializeField] FishingController    fishing;
    [SerializeField] StaminaController    stamina;
    [SerializeField] BlinkAnimationModule blink;

    const float lineH  = 20f;
    const float margin = 6f;
    readonly GUIStyle style = new GUIStyle();

    void Awake()
    {
        style.normal.textColor = Color.white;
        style.fontSize = 14;

        // 若 Inspector 未指定，嘗試自動尋找
        if (!fishing) fishing = FindObjectOfType<FishingController>();
        if (!stamina) stamina = FindObjectOfType<StaminaController>();
        if (!blink)   blink   = FindObjectOfType<BlinkAnimationModule>();
    }

    void OnGUI()
    {
        float y = margin;

        // ───── 釣魚系統 Debug ─────
        if (fishing)
        {
            GUI.Label(new Rect(margin, y, 350, lineH),
                      $"[釣魚] 狀態 = {fishing.CurrentID}", style);
            y += lineH;

            if (fishing.CurrentID == FishingController.StateID.Fishing &&
                fishing.FishingStateRef != null)
            {
                var fs = fishing.FishingStateRef;
                float total   = fs.WaitTotal;
                float remain  = Mathf.Max(fs.WaitRemaining, 0f);
                float elapsed = Mathf.Clamp(total - remain, 0f, total);

                GUI.Label(new Rect(margin, y, 350, lineH),
                          $"  本輪需等待：{total:F1}s", style);
                y += lineH;
                GUI.Label(new Rect(margin, y, 350, lineH),
                          $"  已經過時間：{elapsed:F1}s", style);
                y += lineH;
            }
        }

        // ───── 體力 Debug ─────
        if (stamina)
        {
            float pct = stamina.Current / stamina.Max * 100f;
            GUI.Label(new Rect(margin, y, 350, lineH),
                      $"[體力] {stamina.Current:F1} / {stamina.Max}  ({pct:F0}%)", style);
            y += lineH;
            GUI.Label(new Rect(margin, y, 350, lineH),
                      $"  階段 = {stamina.CurrentID}", style);
            y += lineH;
        }

        // ───── 眨眼 Debug ─────
        if (blink)
        {
            GUI.Label(new Rect(margin, y, 350, lineH),
                      $"[眨眼] 間隔 = {blink.CurrentInterval:F1}s", style);
            y += lineH;
            GUI.Label(new Rect(margin, y, 350, lineH),
                      $"        速度 = {blink.CurrentSpeed:F1}x", style);
        }
    }
}