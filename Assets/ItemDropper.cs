using UnityEngine;

public class ItemDropper : MonoBehaviour
{
    public LayerMask groundLayer;

    public Transform playerTransform;

    public void Drop(ItemClass item, int quantity, float durability)
    {
        if (item.dropPrefab == null) return;

        Vector3 origin = playerTransform.position + playerTransform.forward * 1.5f + Vector3.up * 5f;

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 10f, groundLayer))
        {
            float offsetY = item.dropPrefab.GetComponent<Renderer>()?.bounds.extents.y ?? 0.5f;
            Vector3 spawnPos = hit.point + Vector3.up * offsetY;

            GameObject obj = Instantiate(item.dropPrefab, spawnPos, Quaternion.Euler(0, Random.Range(0, 360), 0));

            var entity = obj.GetComponent<ItemEntity>();
            if (entity != null)
                entity.Initialize(item, quantity, durability);
        }
    }
}
