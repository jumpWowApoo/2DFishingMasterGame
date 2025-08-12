using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Fishing/Mission")]
public class MissionData : ScriptableObject
{
    public string missionId;
    public string title;
    [TextArea] public string description;
    public List<MissionRequirement> needs = new();
    
    [Header("Rewards")]
    public int rewardGold = 0;
}