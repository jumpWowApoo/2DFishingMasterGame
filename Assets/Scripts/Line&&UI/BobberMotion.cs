using System.Collections;
using UnityEngine;

public class BobberMotion : MonoBehaviour
{
    [SerializeField] private float travelTime = 1.2f;
    [SerializeField] private float arcHeight  = 1.5f;

    public IEnumerator MoveTo(Transform bobber, Vector3 target)
    {
        Vector3 start = bobber.position;
        for (float t = 0; t < 1f; t += Time.deltaTime / travelTime)
        {
            Vector3 pos = Vector3.Lerp(start, target, t);
            pos.y += 4f * arcHeight * t * (1f - t);      // 拋物線 y
            bobber.position = pos;
            yield return null;
        }
        bobber.position = target;
    }
}