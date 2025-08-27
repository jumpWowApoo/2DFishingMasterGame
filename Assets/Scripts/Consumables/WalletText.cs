using UnityEngine;
using UnityEngine.UI;
using Game.Currency;

namespace Game.Consumables.UI
{
    public class WalletText : MonoBehaviour
    {
        [SerializeField] Wallet wallet;
        [SerializeField] Text txt;

        void Awake()
        {
            if (!wallet) wallet = FindObjectOfType<Wallet>(true);
            if (!txt)    txt    = GetComponent<Text>();
        }

        void OnEnable()
        {
            if (wallet != null) wallet.OnGoldChanged += Refresh;
            Refresh(wallet != null ? wallet.Gold : 0);
        }

        void OnDisable()
        {
            if (wallet != null) wallet.OnGoldChanged -= Refresh;
        }

        void Refresh(int gold)
        {
            if (txt) txt.text = $"持有金額：{gold}";
        }
    }
}