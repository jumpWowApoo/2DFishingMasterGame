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
        [SerializeField] RectTransform dragRoot;   // 若空，Awake 會嘗試抓 UIHub.Instance.DragLayer

        Action onClose;

        void Awake()
        {
            if (dragRoot == null && UIHub.Instance != null)
                dragRoot = UIHub.Instance.DragLayer;
        }

        /* 顯示面板並設定關閉回調 */
        public void Bind(FishItem item, Action onClose = null)
        {
            if (item == null) return;

            imgBig.sprite = item.BigImage;
            txtName.text  = item.data.fishName;
            txtDesc.text  = item.data.description;

            /* 初始化大圖拖曳 */
            var drag = imgBig.GetComponent<BigImageDragHandle>() ??
                       imgBig.gameObject.AddComponent<BigImageDragHandle>();
            drag.Init(item, dragRoot);

            /* 關閉按鈕 */
            this.onClose = onClose;
            if (closeBtn)
            {
                closeBtn.onClick.RemoveAllListeners();
                closeBtn.onClick.AddListener(() =>
                {
                    onClose?.Invoke();
                    gameObject.SetActive(false);
                });
            }
        }
    }
}