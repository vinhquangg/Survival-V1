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
            // Spawn object trước, để có thể kiểm tra đúng transform và bounds
            Quaternion randomYRot = Quaternion.Euler(0, Random.Range(0, 360), 0);
            GameObject obj = Instantiate(item.dropPrefab, hit.point, randomYRot);

            // Lấy tất cả renderers để tính bounds
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            Vector3 pivotToBottom = Vector3.zero;

            if (renderers.Length > 0)
            {
                Bounds bounds = renderers[0].bounds;
                for (int i = 1; i < renderers.Length; i++)
                    bounds.Encapsulate(renderers[i].bounds);

                float bottomY = bounds.min.y;
                float pivotY = obj.transform.position.y;
                float offsetY = pivotY - bottomY;
                pivotToBottom = new Vector3(0, offsetY, 0);
            }
            else
            {
                Collider col = obj.GetComponentInChildren<Collider>();
                if(col != null)
                {
                    float offsetY = obj.transform.position.y - col.bounds.min.y;
                    pivotToBottom = new Vector3(0, offsetY, 0);
                }
            }

            // Đặt lại vị trí sao cho chân tiếp đất
            Quaternion groundRot = Quaternion.FromToRotation(Vector3.up, hit.normal);
            Quaternion finalRot = groundRot * randomYRot;

            obj.transform.SetPositionAndRotation(hit.point + pivotToBottom, finalRot);

            var entity = obj.GetComponent<ItemEntity>();
            if (entity != null)
                entity.Initialize(item, quantity, durability);

            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySFX(SoundManager.Instance.dropItemSound);
        }
    }
}
