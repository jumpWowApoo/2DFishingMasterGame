/* MissionUI.cs */
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
        // ★ 若沒指定，自己找一次
        if (missionManager == null)
            missionManager = FindObjectOfType<MissionMgr>(true);

        if (missionManager != null)
        {
            missionManager.OnMissionChanged += BuildUI;
            missionManager.OnMissionComplete += HandleMissionComplete;
            // 先等一幀，讓 MissionMgr/Awake/Reset 先跑完
            StartCoroutine(EnsureMissionAssignedThenBuild());
        }
        else
        {
            ShowUpdating();
        }
    }

    System.Collections.IEnumerator EnsureMissionAssignedThenBuild()
    {
        yield return null; // 等一幀，讓 LevelInitializer/Reset 完成

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
            // ★ 叫管理器重播一次（晚訂閱也能拿到）
            missionManager.Rebroadcast();

            // 再看一次
            if (missionManager.Current != null) BuildUI(missionManager.Current);
            else ShowUpdating();
        }
    }

    void OnDisable()
    {
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

    public void Hide() { if (panelMain) panelMain.SetActive(false); }
}
