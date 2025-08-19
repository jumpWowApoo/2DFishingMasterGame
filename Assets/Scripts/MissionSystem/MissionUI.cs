/* MissionUI.cs */
using System.Collections;
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

    void BuildUI(MissionData data)
    {
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
        Dictionary<string,int> counts = new();
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

        btnConfirm.interactable = ready;
        blocker.SetActive(!ready);
    }

    void OnClickConfirm()
    {
        var delivered = slots.Select(s => s.HeldItem).Where(it => it != null).ToList();

        // ★ 結算同步：任務交付的每條魚，從魚箱扣掉 1
        foreach (var item in delivered)
            FishCrate.I.Remove(item.data, 1);

        missionManager.Submit(delivered);
    }

    void HandleMissionComplete()
    {
        txtTitle.gameObject.SetActive(false);
        txtDesc.gameObject.SetActive(false);
        slotHolder.gameObject.SetActive(false);
        txtUpdating.gameObject.SetActive(true);
    }

    public void Hide() { panelMain.SetActive(false); }
}
