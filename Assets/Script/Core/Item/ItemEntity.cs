using UnityEngine;

public class ItemEntity : MonoBehaviour
{
    [SerializeField] private ItemClass itemData;
    [SerializeField] private int quantity;
    [SerializeField] private float durability;

    public void Initialize(ItemClass item, int qty, float dura = -1f)
    {
        itemData = item;
        quantity = qty;
        durability = dura;
    }

    public ItemClass GetItemData() => itemData;
    public int GetQuantity() => quantity;
    public float GetDurability() => durability;
}
