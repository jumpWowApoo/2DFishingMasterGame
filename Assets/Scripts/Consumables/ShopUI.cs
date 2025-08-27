using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Currency; // 你的 Wallet

namespace Game.Consumables.Shop
{
    public class ShopUI : MonoBehaviour
    {
        [Header("資料")]
        [SerializeField] ShopDatabase database;
        [SerializeField] Wallet      wallet;
        [SerializeField] ConsumableBag bag;
        [SerializeField] ShopDelivery delivery; // 分離配送職責

        [Header("中欄：商品清單")]
        [SerializeField] Transform    listRoot;
        [SerializeField] ShopListRow  rowPrefab;

        [Header("上欄：詳細/數量")]
        [SerializeField] Image     detailIcon;
        [SerializeField] Text      detailName;
        [SerializeField] Text      detailPrice;
        [SerializeField] Text      detailDesc;
        [SerializeField] Button    btnMinus;
        [SerializeField] Button    btnPlus;
        [SerializeField] InputField inputQty;
        [SerializeField] Button    btnSetQty; // 覆寫數量

        [Header("下欄：總金額/結帳")]
        [SerializeField] Text   txtTotal;
        [SerializeField] Text   txtWallet;
        [SerializeField] Button btnCheckout;
        [SerializeField] Button btnClose;

        readonly ShopCart _cart = new();
        readonly List<ShopListRow> _rows = new();

        ConsumableData _selected;
        int _editQty = 1;

        void Awake()
        {
            if (!delivery) delivery = GetComponent<ShopDelivery>();

            if (btnMinus)  btnMinus.onClick.AddListener(() => SetEditQty(_editQty - 1));
            if (btnPlus)   btnPlus .onClick.AddListener(() => SetEditQty(_editQty + 1));
            if (inputQty)  inputQty.onEndEdit.AddListener(s =>
            {
                if (int.TryParse(s, out var n)) SetEditQty(n);
                else inputQty.text = _editQty.ToString();
            });

            if (btnSetQty)  btnSetQty .onClick.AddListener(ApplyEditQtyToCart);
            if (btnCheckout) btnCheckout.onClick.AddListener(Checkout);
            if (btnClose)    btnClose   .onClick.AddListener(() => gameObject.SetActive(false));
        }

        void OnEnable()
        {
            if (wallet) wallet.OnGoldChanged += OnGoldChanged;
            BuildList();
            AutoSelectFirst();
            RefreshTotals();
            RefreshWallet();
            RefreshCheckoutInteractable();
        }
        void HighlightSelectedRow()
        {
            foreach (var r in _rows)
                r.SetSelected(r.Data == _selected);
        }

        void OnDisable()
        {
            if (wallet) wallet.OnGoldChanged -= OnGoldChanged;
        }

        void OnGoldChanged(int _) { RefreshWallet(); RefreshCheckoutInteractable(); }

        // ===== 中欄 =====
        void BuildList()
        {
            _rows.Clear();
            foreach (Transform c in listRoot) Destroy(c.gameObject);
            if (database == null) return;

            foreach (var ci in database.items)
            {
                var row = Instantiate(rowPrefab, listRoot);
                int currentQty = _cart.TryGet(ci.data.itemId, out var line) ? line.quantity : 0;
                row.Bind(ci.data, currentQty, this);
                _rows.Add(row);
            }
        }

        void AutoSelectFirst()
        {
            if (database != null && database.Count > 0 && database.Get(0) != null)
                Select(database.Get(0).data);
            else
                Select(null);
        }

        public void Select(ConsumableData data)
        {
            _selected = data;
            if (data == null)
            {
                SetDetail(null, 0, "—", "");
                SetEditQty(1);
                if (btnSetQty) btnSetQty.interactable = false;
                return;
            }

            int unitPrice = FindUnitPrice(data);
            SetDetail(data.icon, unitPrice, data.itemName, data.description);

            int currentQty = _cart.TryGet(data.itemId, out var line) ? line.quantity : 1;
            SetEditQty(currentQty);

            if (btnSetQty) btnSetQty.interactable = true;
            
            HighlightSelectedRow();
        }

        int FindUnitPrice(ConsumableData d)
        {
            foreach (var ci in database.items)
                if (ci.data == d) return ci.Price;
            return d != null ? d.buyPrice : 0;
        }

        void SetDetail(Sprite icon, int unitPrice, string name, string desc)
        {
            if (detailIcon)  { detailIcon.sprite = icon; detailIcon.enabled = (icon != null); }
            if (detailName)  detailName.text  = name ?? "—";
            if (detailPrice) detailPrice.text = $"單價：{unitPrice}";
            if (detailDesc)  detailDesc.text  = desc ?? "";
        }

        void SetEditQty(int n)
        {
            _editQty = Mathf.Clamp(n, 0, 99);
            if (inputQty) inputQty.text = _editQty.ToString();
        }

        void ApplyEditQtyToCart()
        {
            if (_selected == null) return;
            int unitPrice = FindUnitPrice(_selected);
            _cart.SetQuantity(_selected, unitPrice, _editQty); // 覆寫（0=移除）

            BuildList();
            // 逐列更新數量（或直接重建清單也可）
            foreach (var r in _rows)
            {
                int q = _cart.TryGet(r.Data.itemId, out var line) ? line.quantity : 0;
                r.SetQty(q);
            }
            HighlightSelectedRow();

            RefreshTotals();
            RefreshCheckoutInteractable();
        }

        void RefreshTotals()
        {
            if (txtTotal) txtTotal.text = $"總金額：{_cart.Total()}";
        }

        void RefreshWallet()
        {
            if (wallet && txtWallet) txtWallet.text = $"持有金額：{wallet.Gold}";
        }

        void RefreshCheckoutInteractable()
        {
            if (!btnCheckout) return;
            int total = _cart.Total();
            bool can = !_cart.IsEmpty && wallet != null && wallet.Gold >= total;
            btnCheckout.interactable = can;
        }

        // ===== 結帳：一次扣錢，配送到 bag =====
        void Checkout()
        {
            if (wallet == null || bag == null || delivery == null) return;

            int total = _cart.Total();
            if (total <= 0) return;

            if (!wallet.TrySpend(total))
            {
                Debug.Log("[Shop] 餘額不足");
                return;
            }

            int delivered = delivery.DeliverAll(_cart, bag);
            _cart.Clear();

            BuildList();
            RefreshTotals();
            RefreshWallet();
            RefreshCheckoutInteractable();

            Debug.Log($"[Shop] 結帳成功，送入背包 {delivered} 件");
        }
        
        
    }
    
    
}
