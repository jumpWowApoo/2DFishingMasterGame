using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Currency;

namespace Game.Consumables.Shop
{
    public class ShopUI : MonoBehaviour
    {
        [Header("資料")]
        [SerializeField] ShopDatabase database;
        [SerializeField] ConsumableBag bag;
        [SerializeField] ShopDelivery delivery;

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
        [SerializeField] Button    btnSetQty;

        [Header("下欄：總金額/結帳")]
        [SerializeField] Text   txtTotal;
        [SerializeField] Button btnCheckout;
        [SerializeField] Button btnClose;

        readonly ShopCart _cart = new();
        readonly List<ShopListRow> _rows = new();

        ConsumableData _selected;
        int _editQty = 1;

        // 事件綁定用：目前綁到的那顆 Wallet（可能是 Instance，也可能是場上找到的備援）
        Wallet _boundWallet;
        Wallet _fallbackWallet; // 若 Instance 為 null，會以 FindObjectOfType 找到一顆暫時用

        Wallet CurrentWallet => Wallet.Instance ?? _fallbackWallet;

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

            if (btnSetQty)   btnSetQty .onClick.AddListener(ApplyEditQtyToCart);
            if (btnCheckout) btnCheckout.onClick.AddListener(Checkout);
            if (btnClose)    btnClose   .onClick.AddListener(() => gameObject.SetActive(false));

            BindWallet(); // 先嘗試綁定錢包（跨場景單例）
        }

        void OnEnable()
        {
            BindWallet();     // 場景切回來時再確認一次訂閱
            BuildList();
            AutoSelectFirst();
            RefreshTotals();
            RefreshCheckoutInteractable();
        }

        void OnDisable()
        {
            if (_boundWallet) _boundWallet.OnGoldChanged -= OnGoldChanged;
            _boundWallet = null;
        }

        // ========= 綁定/更新錢包事件（無序列化欄位版） =========
        void BindWallet()
        {
            // 先用單例；若暫時沒有單例，退而求其次找場上錢包物件
            var w = Wallet.Instance;
            if (w == null)
            {
                _fallbackWallet ??= FindObjectOfType<Wallet>(true);
                w = _fallbackWallet;
            }

            // 沒有任何錢包：直接退出（UI 的可結帳狀態會被關閉）
            if (w == null)
            {
                RefreshCheckoutInteractable();
                return;
            }

            // 已經綁的是同一顆就不重複
            if (_boundWallet == w) return;

            // 換錢包時，先解除舊訂閱
            if (_boundWallet) _boundWallet.OnGoldChanged -= OnGoldChanged;

            _boundWallet = w;
            _boundWallet.OnGoldChanged += OnGoldChanged;
        }

        void OnGoldChanged(int _)
        {
            RefreshCheckoutInteractable();
        }

        // ===== 中欄 =====
        void BuildList()
        {
            _rows.Clear();
            if (!listRoot) return;
            foreach (Transform c in listRoot) Destroy(c.gameObject);
            if (database == null || rowPrefab == null) return;

            foreach (var ci in database.items)
            {
                var row = Instantiate(rowPrefab, listRoot);
                int currentQty = _cart.TryGet(ci.data.itemId, out var line) ? line.quantity : 0;
                row.Bind(ci.data, currentQty, this);
                _rows.Add(row);
            }
            HighlightSelectedRow();
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
                HighlightSelectedRow();
                return;
            }

            int unitPrice = FindUnitPrice(data);
            SetDetail(data.icon, unitPrice, data.itemName, data.description);

            int currentQty = _cart.TryGet(data.itemId, out var line) ? line.quantity : 1;
            SetEditQty(currentQty);

            if (btnSetQty) btnSetQty.interactable = true;
            HighlightSelectedRow();
        }

        int FindUnitPrice(ConsumableData d) => (d != null) ? d.buyPrice : 0;

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
            _cart.SetQuantity(_selected, unitPrice, _editQty); // 0=移除

            BuildList();
            RefreshTotals();
            RefreshCheckoutInteractable();
        }

        void HighlightSelectedRow()
        {
            foreach (var r in _rows)
                r.SetSelected(r.Data == _selected);
        }

        void RefreshTotals()
        {
            if (txtTotal) txtTotal.text = $"總金額：{_cart.Total()}";
        }

        void RefreshCheckoutInteractable()
        {
            if (!btnCheckout) return;
            int total = _cart.Total();
            var w = CurrentWallet;
            bool can = !_cart.IsEmpty && w != null && w.Gold >= total && total > 0;
            btnCheckout.interactable = can;
        }

        // ===== 結帳 =====
        void Checkout()
        {
            var w = CurrentWallet;
            if (w == null || bag == null || delivery == null) return;

            int total = _cart.Total();
            if (total <= 0) return;

            if (!w.TrySpend(total))
            {
                Debug.Log("[Shop] 餘額不足");
                return;
            }

            int delivered = delivery.DeliverAll(_cart, bag);
            _cart.Clear();

            BuildList();
            RefreshTotals();
            RefreshCheckoutInteractable();

            Debug.Log($"[Shop] 結帳成功，送入背包 {delivered} 件");
        }
    }
}
