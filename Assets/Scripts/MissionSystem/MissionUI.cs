using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Game.Inventory;

public class MissionUI : MonoBehaviour
{
    [Header("UI 物件")] [SerializeField] GameObject panelMain;
    [SerializeField] Text txtTitle;
    [SerializeField] Text txtDesc;
    [SerializeField] Transform slotHolder;
    [SerializeField] Button btnConfirm;
    [SerializeField] GameObject blocker;
    [SerializeField] Text txtUpdating;

    [Header("Prefab")] [SerializeField] MissionSlotUI slotPrefab;

    readonly List<MissionSlotUI> slots = new();
    [SerializeField] MissionMgr missionManager;

    void OnEnable()
    {
        if (missionManager == null)
            missionManager = FindObjectOfType<MissionMgr>(true);

        if (missionManager != null)
        {
            missionManager.OnMissionChanged += BuildUI;
            missionManager.OnMissionComplete += HandleMissionComplete;
            StartCoroutine(EnsureMissionAssignedThenBuild());
        }
        else
        {
            ShowUpdating();
        }
    }

    System.Collections.IEnumerator EnsureMissionAssignedThenBuild()
    {
        yield return null;

        if (missionManager == null)
        {
            ShowUpdating();
            yield break;
        }

        if (missionManager.Current != null)
        {
            BuildUI(missionManager.Current);
        }
        else
        {
            missionManager.Rebroadcast();
            if (missionManager.Current != null) BuildUI(missionManager.Current);
            else ShowUpdating();
        }
    }

    void OnDisable()
    {
        // ★★ 關閉視窗時，先把任務格的魚退回（優先背包，背包滿則退魚箱）
        ReturnAllSlotsBack();

        if (missionManager != null)
        {
            missionManager.OnMissionChanged -= BuildUI;
            missionManager.OnMissionComplete -= HandleMissionComplete;
        }
    }

    void ShowUpdating()
    {
        panelMain.SetActive(true);
        txtTitle.gameObject.SetActive(false);
        txtDesc.gameObject.SetActive(false);
        slotHolder.gameObject.SetActive(false);
        txtUpdating.gameObject.SetActive(true);
        if (btnConfirm) btnConfirm.interactable = false;
        if (blocker) blocker.SetActive(true);
    }

    void BuildUI(MissionData data)
    {
        if (data == null) { ShowUpdating(); return; }

        var existing = slotHolder.GetComponentsInChildren<MissionSlotUI>(true).ToList();
        foreach (var s in existing) { s.gameObject.SetActive(false); slots.Remove(s); }

        panelMain.SetActive(true);
        txtTitle.gameObject.SetActive(true);
        txtDesc.gameObject.SetActive(true);
        slotHolder.gameObject.SetActive(true);
        txtUpdating.gameObject.SetActive(false);

        txtTitle.text = data.title;
        txtDesc.text  = data.description;

        foreach (var s in slots) Destroy(s.gameObject);
        slots.Clear();

        int idx = 0;
        foreach (var req in data.needs)
        {
            for (int i = 0; i < req.count; i++, idx++)
            {
                MissionSlotUI slot;
                if (idx < existing.Count) { slot = existing[idx]; slot.gameObject.SetActive(true); }
                else                      { slot = Instantiate(slotPrefab, slotHolder); }

                slot.ResetSlot(req.fishId);
                slot.OnItemChanged -= CheckFilled;
                slot.OnItemChanged += CheckFilled;
                slots.Add(slot);
            }
        }
        btnConfirm.onClick.RemoveAllListeners();
        btnConfirm.onClick.AddListener(OnClickConfirm);

        CheckFilled();
    }

    void CheckFilled()
    {
        if (missionManager == null || missionManager.Current == null)
        {
            if (btnConfirm) btnConfirm.interactable = false;
            if (blocker) blocker.SetActive(true);
            return;
        }

        var counts = new Dictionary<string,int>();
        foreach (var s in slots)
        {
            if (!s.HasItem) continue;
            string id = s.HeldItem.id;
            if (!counts.ContainsKey(id)) counts[id] = 0;
            counts[id]++;
        }

        bool ready = true;
        foreach (var req in missionManager.Current.needs)
        {
            counts.TryGetValue(req.fishId, out int have);
            if (have < req.count) { ready = false; break; }
        }

        if (btnConfirm) btnConfirm.interactable = ready;
        if (blocker) blocker.SetActive(!ready);
    }

    void OnClickConfirm()
    {
        var delivered = slots.Select(s => s.HeldItem).Where(it => it != null).ToList();

        // 提交時同步扣 FishCrate（維持你原本邏輯）
        foreach (var item in delivered)
            FishCrate.I.Remove(item.data, 1);

        missionManager?.Submit(delivered);
    }

    void HandleMissionComplete()
    {
        txtTitle.gameObject.SetActive(false);
        txtDesc.gameObject.SetActive(false);
        slotHolder.gameObject.SetActive(false);
        txtUpdating.gameObject.SetActive(true);
        if (btnConfirm) btnConfirm.interactable = false;
        if (blocker) blocker.SetActive(true);
    }

    public void Hide()
    {
        // ★ 主動關面板時，也先退回
        ReturnAllSlotsBack();
        if (panelMain) panelMain.SetActive(false);
    }

    /// <summary>
    /// 把所有任務格裡的魚退回：優先回背包；背包滿則退回 FishCrate；最後清空格。
    /// </summary>
    void ReturnAllSlotsBack()
    {
        // 先複製一份，避免在迴圈中改動 children
        var allSlots = slotHolder.GetComponentsInChildren<MissionSlotUI>(true);
        foreach (var s in allSlots)
        {
            if (!s || !s.HasItem) continue;

            var item = s.HeldItem;

            // 優先退回背包
            int empty = InventoryMgr.Instance?.FirstEmptySlot() ?? -1;
            if (empty >= 0)
            {
                InventoryMgr.Instance.AddAt(empty, item);
            }
            else
            {
                // 背包滿就退回 FishCrate（避免物品遺失/dup）
                if (FishCrate.I != null && item != null && item.data != null)
                    FishCrate.I.Add(item.data, 1);
            }

            // 清空 UI 格
            s.ResetSlot(item?.id);
        }

        // 關掉確認按鈕 & 上鎖
        if (btnConfirm) btnConfirm.interactable = false;
        if (blocker) blocker.SetActive(true);
    }
}
