using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public Transform cameraTransform;
    public Transform playerHead;
    public float minDistance = 0.1f;
    public LayerMask obstacleLayer;

    void LateUpdate()
    {
        Vector3 direction = (cameraTransform.position - playerHead.position).normalized;
        float distance = Vector3.Distance(cameraTransform.position, playerHead.position);

        if (Physics.Raycast(playerHead.position, direction, out RaycastHit hit, distance, obstacleLayer))
        {
            cameraTransform.position = hit.point - direction * minDistance;
        }
    }
}
