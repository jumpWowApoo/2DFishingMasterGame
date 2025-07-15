using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class FishingLine : MonoBehaviour
{
    private LineRenderer lr;
    private Transform    endA, endB;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 2;
    }

    /* 讓外部可一次性顯示/隱藏整條線 */
    public void Show(bool on) => lr.enabled = on;

    public void SetTargets(Transform a, Transform b)
    {
        endA = a;
        endB = b;
    }

    private void LateUpdate()
    {
        if (!endA || !endB || !lr.enabled) return;
        lr.SetPosition(0, endA.position);
        lr.SetPosition(1, endB.position);
    }
}