using UnityEngine;
using Game.Consumables;
using Game.Session;

public class SettlementReturnHydrator : MonoBehaviour
{
    [Header("A 場景的 7 格工具欄")] [SerializeField]
    CarrySlots carry;

    [Header("A 場景的背包（可選，用來安置占用時的剩餘道具）")] [SerializeField]
    ConsumableBag bag;

    void OnEnable()
    { // 把 B 場景剩餘的裝備（Session.LoadoutSlots）回填到 A 的 7 格；占用則塞進背包
      InventorySync.ReturnSessionLoadoutToA(carry, bag); // 把 Session 中的背包內容灌回 A 的背包
      InventorySync.LoadSessionBagToA(bag); } }