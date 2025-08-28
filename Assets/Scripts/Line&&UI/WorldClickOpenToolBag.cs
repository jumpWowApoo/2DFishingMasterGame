using UnityEngine;

namespace Game.Consumables.UI
{
    /// <summary>
    /// 點擊此物件（需在場景物件上有 Collider 或 Collider2D）
    /// 來開啟/關閉指定的工具背包面板（例如 B 場景的 EquipBarPanel）。
    /// 也提供外部呼叫的 Open/Close/Toggle。
    /// </summary>
    public class WorldClickOpenToolBag : MonoBehaviour
    {
        [Header("要開啟/關閉的面板（例如：EquipBarPanel）")]
        [SerializeField] GameObject toolBagPanel;

        // 滑鼠在此物件的 Collider 上按下時觸發
        void OnMouseDown()
        {
            if (!toolBagPanel) return;
            toolBagPanel.SetActive(!toolBagPanel.activeSelf);
        }

        /// <summary>外部也可以呼叫開啟</summary>
        public void Open()
        {
            if (!toolBagPanel) return;
            toolBagPanel.SetActive(true);
        }

        /// <summary>外部也可以呼叫關閉</summary>
        public void Close()
        {
            if (!toolBagPanel) return;
            toolBagPanel.SetActive(false);
        }

        /// <summary>外部也可以呼叫切換</summary>
        public void Toggle()
        {
            if (!toolBagPanel) return;
            toolBagPanel.SetActive(!toolBagPanel.activeSelf);
        }
    }
}