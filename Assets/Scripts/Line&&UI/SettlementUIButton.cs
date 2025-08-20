using UnityEngine;
using UnityEngine.UI;

public class SettlementUIButton : MonoBehaviour
{
    void Awake()
    {
        var btn = GetComponent<Button>();
    }

    public void OpenSettlement()
    {
        SettlementFlow.OpenSettlement();
    }
}