using System.Collections;
using UnityEngine;

public class TreeInstance : MonoBehaviour, IPoolable
{
    public TreeClass treeData;
    public bool isChopped = false;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    [Header("Log Spawn")]
    public GameObject logDropGO;
    private void Awake()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        if (logDropGO != null) logDropGO.SetActive(false);
    }

    public void ResetState()
    {
        isChopped = false;
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        if (logDropGO != null) logDropGO.SetActive(false);
        gameObject.SetActive(true);
    }

    public void ShowLogDropAfterDelay(float delay)
    {
        StartCoroutine(ShowLogCoroutine(delay));
    }

    private IEnumerator ShowLogCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!string.IsNullOrEmpty(treeData.logPoolID))
        {
            // 🟢 Rơi 2 log
            for (int i = 0; i < 2; i++)
            {
                Vector3 dropOffset = new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f));
                Vector3 spawnPosition = GetGroundPosition(transform.position + Vector3.up * 3f + dropOffset);
                GameObject log = ObjectPoolManager.Instance.SpawnFromPool(treeData.logPoolID, spawnPosition, Quaternion.identity);

                if (log == null)
                    Debug.LogWarning("❌ Không spawn được log từ poolID: " + treeData.logPoolID);
            }
        }
        else
        {
            Debug.LogWarning("❌ treeData.logPoolID trống, không thể spawn log");
        }
    }



    private Vector3 GetGroundPosition(Vector3 origin)
    {
        RaycastHit hit;
        if (Physics.Raycast(origin, Vector3.down, out hit, 10f))
        {
            return hit.point; // Vị trí mặt đất
        }
        return origin; // fallback nếu không có ground
    }

    public void OnSpawned()
    {
        if (isChopped)
        {
            // Nếu cây đã chặt, tắt cây và spawn gốc
            gameObject.SetActive(false);
            //if (!string.IsNullOrEmpty(treeData.stumpPoolID))
            //{
            //    ObjectPoolManager.Instance.SpawnFromPool(treeData.stumpPoolID, transform.position, transform.rotation);
            //}
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

    public void OnReturned()
    {
        // Khi trả về pool, luôn tắt object
        gameObject.SetActive(false);
    }
}

