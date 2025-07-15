using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFishingState
{
    void OnEnter();
    void Tick();
    void OnExit();
}
