using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGUI : MonoBehaviour
{
    [SerializeField] FishingController controller; 
    void OnGUI()
    {
        if (!controller) return;
        const int margin = 4;
        float lineH = 22f; ;
        float w = 300f;
        GUI.color = new Color(0, 0, 0, 1f);       // 半透明黑
        
        Rect r = new Rect(margin, margin, 320, 32);
        string txt = $"目前狀態： {controller.CurrentID}";
        GUI.Label(new Rect(10, 10, 300, 30), txt);
        
        if (controller.CurrentID == FishingController.StateID.Fishing &&
            controller.FishingStateRef != null)
        {
            var fs = controller.FishingStateRef;

            // 總秒數/剩餘/已過 -- Clamp 防負值
            float total = fs.WaitTotal;
            float remain = Mathf.Max(fs.WaitRemaining, 0f);
            float elapsed = Mathf.Clamp(total - remain, 0f, total);

            GUI.Label(new Rect(margin, margin + lineH, w, lineH),
                $"本輪需等待： {total:F1} 秒");

            GUI.Label(new Rect(margin, margin + 2*lineH, w, lineH),
                $"已經過時間： {elapsed:F1} 秒");
        }
    }
}

