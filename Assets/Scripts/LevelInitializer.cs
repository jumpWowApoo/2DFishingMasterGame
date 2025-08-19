using System.Collections;
using System.Linq;
using UnityEngine;
using Game.Common;

public class LevelInitializer : MonoBehaviour
{
    void Start() { StartCoroutine(DoReset()); }

    IEnumerator DoReset()
    {
        Time.timeScale = 1f;

        if (SceneReturnContext.Reset == ResetLevel.None)
            yield break;

        yield return null; // 等一幀，讓 Awake/Start 都完成

        var allBehaviours = Resources.FindObjectsOfTypeAll<MonoBehaviour>();
        var resettables   = allBehaviours.OfType<IResettable>().ToList();
        foreach (var r in resettables)
            r.ResetForNewRound(SceneReturnContext.Reset);

        // （可留可移除）保險清理
        var sta = Game.Stamina.StaminaController.Instance;
        if (sta != null) sta.ChangeStamina(sta.Max - sta.Current);
        Game.Inventory.InventoryMgr.Instance?.Clear();
        FishCrate.I?.Clear();

        SceneReturnContext.Reset = ResetLevel.None;
    }
}