using System.Collections;
using UnityEngine;

public class TreeInstance : MonoBehaviour
{
    public TreeClass treeData;
    public bool isChopped = false;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    [Header("Log Spawn")]
    public GameObject logDropGO; // Kéo logDrop child vào đây trong prefab

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
            //logDropGO.transform.position = transform.position; // Hoặc position + offset
            GameObject spawnedLog = Instantiate(logDropGO, transform.position, Quaternion.identity);
            logDropGO.SetActive(true);
        }
    }
}
