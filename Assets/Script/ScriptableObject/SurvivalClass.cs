using UnityEngine;
[CreateAssetMenu(fileName = "NewSurvivalItem", menuName = "Item/Survival")]
public class SurvivalClass : ItemClass
{
    public GameObject prefabToPlace;   
    public bool requiresFuel;          
    public float duration;             
    public bool canBeTurnedOn;
    [Header("Placement Colors")]
    public Material previewMaterial;  
    public Material validMaterial;    
    public Material invalidMaterial;  
    public Material originalMaterial;

    [Header("Required Items to Build")]
    public RequiredItem[] requiredItems;
    public override ItemClass GetItem() { return this; }
    public override SurvivalClass GetSurvival() { return this; }
    public override ToolClass GetTool() { return null; }
    public override MiscClass GetMisc() { return null; }
    public override Consumable GetConsumable() { return null; }
    public override WeaponClass GetWeapon() { return null; }
    public override float GetDurability() => -1f;




    [System.Serializable]
    public class RequiredItem
    {
        public ItemClass item;  // Item cần để xây
        public int amount;      // Số lượng cần
    }
}
