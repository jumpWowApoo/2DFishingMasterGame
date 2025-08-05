using UnityEngine;
using Game.Stamina;

/// <summary>
/// 測試眨眼：數字鍵 1‧2‧3‧4 控制不同間隔 / 速度
/// 按 0 ＝ 停止眨眼
/// </summary>
public class BlinkTester : MonoBehaviour
{
    [SerializeField] BlinkAnimationModule blink;

    void Awake()
    {
        if (!blink) blink = FindObjectOfType<BlinkAnimationModule>();
    }

    void Update()
    {
        if (!blink) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) // 1
            blink.SetBlink(12f, 1f);          // 每 10s‧正常速
        if (Input.GetKeyDown(KeyCode.Alpha2)) // 2
            blink.SetBlink(5f, 1.5f);         // 每 5s‧1.5×
        if (Input.GetKeyDown(KeyCode.Alpha3)) // 3
            blink.SetBlink(5f, 2f);           // 每 2s‧2×
        if (Input.GetKeyDown(KeyCode.Alpha4)) // 4
            blink.SetBlink(5f, 3f);         // 每 1s‧0.5×
        if (Input.GetKeyDown(KeyCode.Alpha0)) // 0
            blink.SetBlink(0f, 0f);           // 停止眨眼
    }
}