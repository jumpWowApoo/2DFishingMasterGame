using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class FishInfoPanel : MonoBehaviour
    {
        [SerializeField] Image imgBig;
        [SerializeField] Text  txtName;
        [SerializeField] Text  txtDesc;
        [SerializeField] Button closeBtn;

        Action onClose;

        /// <summary>顯示面板並設定關閉回調</summary>
        public void Bind(FishItem item, Action onClose = null)
        {
            if (item == null) return;

            imgBig.sprite = item.BigImage;
            txtName.text  = item.data.fishName;
            txtDesc.text  = item.data.description;

            var drag = imgBig.GetComponent<BigImageDragHandle>() ??
                       imgBig.gameObject.AddComponent<BigImageDragHandle>();
            drag.Init(item);

            this.onClose = onClose;

            if (closeBtn)
            {
                closeBtn.onClick.RemoveAllListeners();
                closeBtn.onClick.AddListener(Close); // 統一走 Close()
            }
        }

        /// <summary>統一關閉入口：先觸發 onClose，再關閉面板</summary>
        public void Close()
        {
            try { onClose?.Invoke(); }
            finally { gameObject.SetActive(false); }
        }
    }
}