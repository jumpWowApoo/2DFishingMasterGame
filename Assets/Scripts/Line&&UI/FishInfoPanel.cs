using System;
using UnityEngine;
using UnityEngine.UI;

    /// <summary>魚資料面板（顯示大圖＋說明）。</summary>
    public class FishInfoPanel : MonoBehaviour
    {
        [SerializeField] Image    imgBig;
        [SerializeField] Text txtName;
        [SerializeField] Text txtDesc;
        [SerializeField] Button closeBtn; 
        
        Action onClose;  
        
        public void Bind(FishItem item, Action onClose = null)
        {
            imgBig.sprite = item.BigImage;
            txtName.text  = item.data.fishName;
            txtDesc.text  = item.data.description;

            this.onClose = onClose;
            if (closeBtn)                     // 保險起見
            {
                closeBtn.onClick.RemoveAllListeners();
                closeBtn.onClick.AddListener(() =>
                {
                    onClose?.Invoke();
                    gameObject.SetActive(false);  // 或 Destroy(gameObject);
                });
            }
        }
        
    }
