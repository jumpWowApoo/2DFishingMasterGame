using UnityEngine;

namespace Game.Consumables.Shop
{
    public class ShopCloseOnEscape : MonoBehaviour
    {
        [SerializeField] GameObject shopPanel;

        void Update()
        {
            if (shopPanel && shopPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
                shopPanel.SetActive(false);
        }
    }
}