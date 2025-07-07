using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Crafting/Blueprint")]
public class BlueprintData : ScriptableObject
{
    public ItemClass resultItem;
    public int resultAmount = 1;

    [System.Serializable]
    public class Requirement
    {
        public ItemClass item;
        public int amount;
    }

    public List<Requirement> requirements;
}
