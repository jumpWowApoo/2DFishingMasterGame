using UnityEngine;
using UnityEngine.SceneManagement;
using Game.Consumables;
using Game.Session;

[DisallowMultipleComponent]
public class ASceneGateway : MonoBehaviour
{
    [Header("A 場景資料容器")]
    [SerializeField] CarrySlots    carry;   // A 的 7 格
    [SerializeField] ConsumableBag bag;     // A 的 28 格

    [Header("進場行為")]
    [SerializeField] bool loadBagFromSessionOnStart  = true; // ❶ 先載入 Session 背包
    [SerializeField] bool returnLoadoutOnStart       = true; // ❷ 再回填 B 剩餘裝備
    [SerializeField] bool saveBackToSessionOnStart   = true; // ❸ 最後存回 Session，避免 UI 還沒存時丟失
    [SerializeField] bool onlyLoadBagIfSessionHasAny = true; // Session 背包有東西才覆蓋

    [Header("偵錯")]
    [SerializeField] bool logVerbose = true;

    static bool _hookedSceneEvent;
    bool _savedOnce;
    bool _hydratedOnce;

    void ResolveRefs()
    {
        if (!carry) carry = FindObjectOfType<CarrySlots>(true);
        if (!bag)   bag   = FindObjectOfType<ConsumableBag>(true);
    }

    void Awake()
    {
        ResolveRefs();
        if (!_hookedSceneEvent)
        {
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            _hookedSceneEvent = true;
        }
    }

    void Start()
    {
        if (_hydratedOnce) return;
        ResolveRefs();

        // ❶ 先讓 A 背包等於 Session 背包（避免之後回填的東西被覆蓋掉）
        if (loadBagFromSessionOnStart && bag &&
            (!onlyLoadBagIfSessionHasAny || SessionInventory.HasAny(SessionInventory.BagSlots)))
        {
            InventorySync.LoadSessionBagToA(bag);
            if (logVerbose) Debug.Log("[A-Gateway] 覆蓋：Session.Bag → A背包");
        }

        // ❷ 再把 B 的 Loadout 剩餘裝備回填回來（7格空位優先，其次進背包）
        if (returnLoadoutOnStart && (carry || bag) && SessionInventory.HasAny(SessionInventory.LoadoutSlots))
        {
            int before = SessionInventory.CountNonNull(SessionInventory.LoadoutSlots);
            InventorySync.ReturnSessionLoadoutToA(carry, bag, verbose: logVerbose);
            if (logVerbose) Debug.Log($"[A-Gateway] 回填：Session.Loadout → A的7格/背包（{before} 件）");
        }

        // ❸ 把「回填後的 A 狀態」寫回 Session，之後再切回 B 就讀到正確內容
        if (saveBackToSessionOnStart)
        {
            InventorySync.SaveAtoSession(carry, bag);
            if (logVerbose) Debug.Log("[A-Gateway] 開場保存：A → Session（寫回 7格與背包）");
        }

        _hydratedOnce = true;
    }

    // —— 只在「換場」時保存（多重保險）——
    void OnDisable()               { TrySave("[A-Gateway] OnDisable 儲存 A → Session"); }
    void OnDestroy()               { TrySave("[A-Gateway] OnDestroy 儲存 A → Session"); }
    void OnApplicationQuit()       { TrySave("[A-Gateway] Quit 儲存 A → Session"); }
    void OnApplicationPause(bool p){ if (p) TrySave("[A-Gateway] Pause 儲存 A → Session"); }

    static void OnSceneUnloaded(Scene s)
    {
        var carry = Object.FindObjectOfType<CarrySlots>(true);
        var bag   = Object.FindObjectOfType<ConsumableBag>(true);
        if (carry || bag)
        {
            InventorySync.SaveAtoSession(carry, bag);
            Debug.Log("[A-Gateway] sceneUnloaded 儲存 A → Session");
        }
    }

    void TrySave(string why)
    {
        if (_savedOnce) return;
        ResolveRefs();
        InventorySync.SaveAtoSession(carry, bag);
        _savedOnce = true;
        if (logVerbose) Debug.Log(why);
    }

    // （可選）切場前手動保險
    public void ForceSaveANow()
    {
        ResolveRefs();
        InventorySync.SaveAtoSession(carry, bag);
        if (logVerbose) Debug.Log("[A-Gateway] ForceSaveANow 儲存 A → Session");
    }
}
