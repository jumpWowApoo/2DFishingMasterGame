using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Fishing/Fish Database")]
public class FishDatabase : ScriptableObject
{
    public List<FishData> fishes;
}