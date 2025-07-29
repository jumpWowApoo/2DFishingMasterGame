using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 釣魚系統的介面
/// </summary>

public interface IFishingState
{
    void OnEnter();
    void Tick();
    void OnExit();
}
