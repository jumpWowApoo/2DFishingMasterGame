using UnityEngine;
using UnityEngine.UI;

namespace Game.Consumables.Shop
{
    /// <summary>
    /// 商品清單的一列：整列是 Button（透明也可），顯示 [icon][name][×qty]。
    /// 點擊後通知 ShopUI.Select()。
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class ShopListRow : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] Image icon;
        [SerializeField] Text  txtName;
        [SerializeField] Text  txtQty;          // 顯示購買中的數量（×N）
        [SerializeField] Image selectionBg;     // 可選：被選取時顯示(可半透明)
        [SerializeField] Color selectedColor = new Color(1,1,1,0.15f);

        Button btn;
        ConsumableData bound;
        ShopUI owner;

        void Awake() => btn = GetComponent<Button>();

        public void Bind(ConsumableData data, int currentQty, ShopUI shop)
        {
            bound = data;
            owner = shop;

            if (icon)    { icon.sprite = data.icon; icon.enabled = (data.icon != null); }
            if (txtName) txtName.text = data.itemName;
            SetQty(currentQty);

            if (btn)
            {
                btn.transition = Selectable.Transition.ColorTint; // 透明大按鈕仍有高亮
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => owner.Select(bound));
            }

            SetSelected(false);
        }

        public void SetQty(int q)
        {
            if (txtQty) txtQty.text = $"×{Mathf.Max(0, q)}";
        }

        public void SetSelected(bool on)
        {
            if (!selectionBg) return;
            selectionBg.enabled = on;
            selectionBg.color   = selectedColor;
        }

        // 讓 ShopUI 在改數量後更新這一列的 Qty
        public ConsumableData Data => bound;
    }
}