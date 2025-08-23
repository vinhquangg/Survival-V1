using System.Collections;
using UnityEngine;

public class TreeChopInteraction : MonoBehaviour, IInteractable, IInteractableInfo
{
    private TreeInstance treeInstance;

    private void Start()
    {
        treeInstance = GetComponent<TreeInstance>();
    }

    public Sprite GetIcon()
    {
        return null;
    }

    public InteractionType GetInteractionType()
    {
        return InteractionType.Chop;
    }

    public string GetItemAmount()
    {
        return "";
    }

    public string GetName()
    {
        if (treeInstance == null)
            treeInstance = GetComponent<TreeInstance>();

        return "Chop " + (treeInstance?.treeData?.treeName ?? "Unknown");
    }

    public void Interact(GameObject interactor)
    {
        var player = interactor.GetComponent<PlayerController>();
        if (player != null)
        {
            // 👉 gọi xoay mượt trước khi vào state Chop
            //player.RotateTowards(transform.position, 8f);

            // 👉 đổi state sau đó
            player.playerStateMachine.ChangeState(new ChopState(player.playerStateMachine, player, this));
        }
    }


    public void OnChopped()
    {
        Debug.Log("🌳 Cây bị chặt rồi!");

        if (treeInstance == null)
            treeInstance = GetComponent<TreeInstance>();

        if (treeInstance != null)
        {
            treeInstance.isChopped = true;

            // 🌱 Spawn gốc cây từ Pool
            if (!string.IsNullOrEmpty(treeInstance.treeData.stumpPoolID))
            {
                ObjectPoolManager.Instance.SpawnFromPool(
                    treeInstance.treeData.stumpPoolID,
                    transform.position,
                    transform.rotation
                );
            }

            // 🪵 Spawn LogDrop từ Pool
            if (!string.IsNullOrEmpty(treeInstance.treeData.logPoolID))
            {
                // Lấy chiều cao từ collider để dịch object lên trên
                float offsetY = 0.5f;
                if (treeInstance.logDropGO != null && treeInstance.logDropGO.TryGetComponent<Collider>(out var logCol))
                {
                    offsetY = logCol.bounds.extents.y; // nửa chiều cao của log
                }

                // 👉 dịch sang phải một chút + dịch lên để không chìm đất
                Vector3 spawnPos = GetGroundPosition(
                    transform.position + Vector3.up * 3f,
                    treeInstance.logDropGO
                ) + transform.right * 1.5f + Vector3.up * offsetY;

                GameObject logDrop = ObjectPoolManager.Instance.SpawnFromPool(
                    treeInstance.treeData.logPoolID,
                    spawnPos,
                    Quaternion.identity
                );

                if (logDrop != null && logDrop.TryGetComponent<Rigidbody>(out var rb))
                {
                    rb.isKinematic = false;
                    rb.useGravity = true;
                    rb.velocity = Vector3.down * 1f;

                    treeInstance.StartCoroutine(DropAndSettle(rb));
                }
            }


            // ⏱ Trả cây về pool sau khi chặt
            StartCoroutine(ReturnToPoolWithDelay(0.6f));
        }
    }

    private IEnumerator ReturnToPoolWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ObjectPoolManager.Instance.ReturnToPool(gameObject);
    }

    private IEnumerator DropAndSettle(Rigidbody rb)
    {
        yield return new WaitForSeconds(0.5f);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
    }

    private Vector3 GetGroundPosition(Vector3 origin, GameObject prefab = null)
    {
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 10f, LayerMask.GetMask("Ground")))
        {
            Vector3 pos = hit.point;

            if (prefab != null && prefab.TryGetComponent<Collider>(out var col))
            {
                pos.y += col.bounds.extents.y;
            }

            return pos;
        }
        return origin;
    }

}
