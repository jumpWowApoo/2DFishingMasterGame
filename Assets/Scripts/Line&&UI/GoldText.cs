using UnityEngine;
using UnityEngine.UI;
using Game.Currency;

public class GoldText : MonoBehaviour
{
    [SerializeField] Text text;

    void Awake()
    {
        if (!text) text = GetComponent<Text>();
    }

    void OnEnable()
    {
        if (Wallet.Instance != null)
        {
            Wallet.Instance.OnGoldChanged += Refresh;
            Refresh(Wallet.Instance.Gold);
        }
    }

    void OnDisable()
    {
        if (Wallet.Instance != null)
            Wallet.Instance.OnGoldChanged -= Refresh;
    }

    void Refresh(int gold) => text.text = gold.ToString();
}