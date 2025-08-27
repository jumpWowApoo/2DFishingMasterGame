using UnityEngine;

namespace Game.Consumables.Shop
{
    public class ShopOpenButton : MonoBehaviour
    {
        [SerializeField] GameObject shopPanel;

        public void Open()  { if (shopPanel) shopPanel.SetActive(true); }
        public void Close() { if (shopPanel) shopPanel.SetActive(false); }
        public void Toggle(){ if (shopPanel) shopPanel.SetActive(!shopPanel.activeSelf); }
    }
}