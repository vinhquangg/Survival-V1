using System.Collections;
using UnityEngine;

public class TreeInstance : MonoBehaviour
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

        if (logDropGO != null)
        {
            Vector3 spawnPosition = GetGroundPosition(transform.position + Vector3.up * 3f); // ray từ trên cao xuống
            GameObject spawnedLog = Instantiate(logDropGO, spawnPosition, Quaternion.identity);
            spawnedLog.SetActive(true);
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


}
