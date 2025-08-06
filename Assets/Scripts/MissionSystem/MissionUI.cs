using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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

    /* ───── 事件綁定 ───── */
    void OnEnable()
    {
        if (missionManager != null)
        {
            missionManager.OnMissionChanged += BuildUI;
            missionManager.OnMissionComplete += HandleMissionComplete;
            if (missionManager.Current != null)
                BuildUI(missionManager.Current);
        }
    }

    void OnDisable()
    {
        missionManager.OnMissionChanged -= BuildUI;
        missionManager.OnMissionComplete -= HandleMissionComplete;
    }

    /* ───── 生成任務格子 ───── */
    void BuildUI(MissionData data)
    {
        List<MissionSlotUI> existing = slotHolder.GetComponentsInChildren<MissionSlotUI>(true).ToList();
        foreach (var s in existing)
        {
            s.gameObject.SetActive(false);
            slots.Remove(s);
        }

        panelMain.SetActive(true);
        txtTitle.gameObject.SetActive(true);
        txtDesc.gameObject.SetActive(true);
        slotHolder.gameObject.SetActive(true);
        txtUpdating.gameObject.SetActive(false);

        txtTitle.text = data.title;
        txtDesc.text = data.description;

        int needTotal = data.needs.Sum(n => n.count);
        int idx = 0;

        foreach (var s in slots) Destroy(s.gameObject);
        slots.Clear();
        foreach (var req in data.needs)
        {
            for (int i = 0; i < req.count; i++, idx++)
            {
                MissionSlotUI slot;

                // 優先重用已存在的
                if (idx < existing.Count)
                {
                    slot = existing[idx];
                    slot.gameObject.SetActive(true);
                }
                else
                {
                    slot = Instantiate(slotPrefab, slotHolder);
                }

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

    /* ───── 驗證填滿 ───── */
    void CheckFilled()
    {
        // 統計當前已放入的魚種與數量
        Dictionary<string,int> counts = new();
        foreach (var s in slots)
        {
            if (!s.HasItem) continue;
            string id = s.HeldItem.id;
            if (!counts.ContainsKey(id)) counts[id] = 0;
            counts[id]++;
        }

        // 驗證是否符合 Mission 需求
        bool ready = true;
        foreach (var req in missionManager.Current.needs)
        {
            counts.TryGetValue(req.fishId, out int have);
            if (have < req.count) { ready = false; break; }
        }

        btnConfirm.interactable = ready;
        blocker.SetActive(!ready);
    }


    /* ───── 按下確認 ───── */
    void OnClickConfirm()
    {
        var delivered = slots.Select(s => s.HeldItem).ToList();
        missionManager.Submit(delivered);
    }

    /* ───── 任務完成 → 更新動畫 ───── */
    void HandleMissionComplete()
    {
        txtTitle.gameObject.SetActive(false);
        txtDesc.gameObject.SetActive(false);
        slotHolder.gameObject.SetActive(false);
        txtUpdating.gameObject.SetActive(true);
    }
    
    public void Hide()
    { panelMain.SetActive(false); }

}