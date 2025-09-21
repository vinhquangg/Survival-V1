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
            if (player.playerStateMachine.currentState is ChopState) return;

            var equipManager = FindObjectOfType<EquipManager>();
            if (equipManager == null || !equipManager.HasItemEquipped(EquipType.Tool))
            {
                var feedback = GameObject.FindObjectOfType<PlayerFeedbackUI>();
                if (feedback != null)
                    feedback.ShowFeedback(FeedbackType.NeedAxe);
                return;
            }

            player.playerStateMachine.ChangeState(new ChopState(player.playerStateMachine, player, this));
        }
    }

    // Gọi tại Animation Event "Impact"
    public void SpawnDrops()
    {
        Debug.Log("🌳 Spawn gỗ + gốc!");

        if (treeInstance == null)
            treeInstance = GetComponent<TreeInstance>();

        if (treeInstance != null && !treeInstance.isChopped)
        {
            treeInstance.isChopped = true;

            // 🌱 Spawn gốc
            if (!string.IsNullOrEmpty(treeInstance.treeData.stumpPoolID))
            {
                ObjectPoolManager.Instance.SpawnFromPool(
                    treeInstance.treeData.stumpPoolID,
                    transform.position,
                    transform.rotation
                );
            }

            // 🪵 Spawn gỗ
            if (!string.IsNullOrEmpty(treeInstance.treeData.logPoolID))
            {
                float offsetY = 0.5f;
                if (treeInstance.logDropGO != null && treeInstance.logDropGO.TryGetComponent<Collider>(out var logCol))
                {
                    offsetY = logCol.bounds.extents.y;
                }

                // 👉 tạo vị trí spawn thử nghiệm (dịch ngang trước)
                Vector3 testPos = transform.position + transform.right * 1.5f + Vector3.up * 3f;

                // 👉 raycast từ vị trí này xuống đất
                Vector3 groundPos = GetGroundPosition(testPos, treeInstance.logDropGO);

                // 👉 cộng thêm offsetY để không bị cắm xuống đất
                Vector3 spawnPos = groundPos + Vector3.up * offsetY;

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
        }
    }

    // Gọi tại Animation Event "End"
    public void HideTree()
    {
        StartCoroutine(ReturnToPoolWithDelay(0.2f));
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

    // Không dùng nữa, chỉ để tham khảo
    //public void OnChopped() => SpawnDrops();
}
