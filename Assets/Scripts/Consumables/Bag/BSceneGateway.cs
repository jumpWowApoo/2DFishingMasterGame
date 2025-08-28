using UnityEngine;
using UnityEngine.SceneManagement;
using Game.Consumables;
using Game.Session;

[DisallowMultipleComponent]
[DefaultExecutionOrder(100)] // 讓 PlayerLoadout.Awake 先跑
public class BSceneGateway : MonoBehaviour
{
    [Header("B 場景的 7 格")]
    [SerializeField] PlayerLoadout loadout;

    [Header("進場行為")]
    [SerializeField] bool seedSessionFromCurrentLoadoutIfEmpty = true; // 冷啟動保底

    [Header("偵錯")]
    [SerializeField] bool logVerbose = true;

    static bool _hookedSceneEvent;
    bool _loadedOnce;
    bool _savedOnce;

    void ResolveRef()
    {
        if (!loadout) loadout = GetComponent<PlayerLoadout>();
        if (!loadout) loadout = FindObjectOfType<PlayerLoadout>(true);
    }

    void Awake()
    {
        ResolveRef();
        if (!_hookedSceneEvent)
        {
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            _hookedSceneEvent = true;
        }
    }

    void Start()
    {
        ResolveRef();
        if (_loadedOnce || !loadout) return;

        if (SessionInventory.HasAny(SessionInventory.CarrySlots))
        {
            InventorySync.LoadSessionToB(loadout);
            if (logVerbose) Debug.Log("[B-Gateway] 載入：Session 7格 → B.PlayerLoadout");
        }
        else if (seedSessionFromCurrentLoadoutIfEmpty)
        {
            InventorySync.SaveBtoSession(loadout);
            if (logVerbose) Debug.Log("[B-Gateway] Session 為空，已以當前 PlayerLoadout 種入 Session（冷啟動保底）");
        }

        _loadedOnce = true;
    }

    // —— 只在「換場」時保存（多重保險）——
    void OnDisable()               { TrySave("[B-Gateway] OnDisable 儲存 B → Session"); }
    void OnDestroy()               { TrySave("[B-Gateway] OnDestroy 儲存 B → Session"); }
    void OnApplicationQuit()       { TrySave("[B-Gateway] Quit 儲存 B → Session"); }
    void OnApplicationPause(bool p){ if (p) TrySave("[B-Gateway] Pause 儲存 B → Session"); }

    static void OnSceneUnloaded(Scene s)
    {
        var loadout = Object.FindObjectOfType<PlayerLoadout>(true);
        if (loadout)
        {
            InventorySync.SaveBtoSession(loadout);
            Debug.Log("[B-Gateway] sceneUnloaded 儲存 B → Session.Loadout");
        }
    }

    void TrySave(string why)
    {
        if (_savedOnce) return;
        ResolveRef();
        if (loadout)
        {
            InventorySync.SaveBtoSession(loadout);
            _savedOnce = true;
            if (logVerbose) Debug.Log(why);
        }
    }

    // （可選）供切場前手動保險存一次
    public void ForceSaveBNow()
    {
        ResolveRef();
        if (loadout) InventorySync.SaveBtoSession(loadout);
        if (logVerbose) Debug.Log("[B-Gateway] ForceSaveBNow 儲存 B → Session");
    }
}
