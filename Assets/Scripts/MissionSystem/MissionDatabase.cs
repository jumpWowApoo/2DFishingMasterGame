/* MissionDatabase.cs */
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Fishing/MissionDatabase")]
public class MissionDatabase : ScriptableObject
{
    public List<MissionData> all = new();
}