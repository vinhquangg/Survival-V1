using UnityEngine;

public class AimTargetFollower : MonoBehaviour
{
    public Transform cameraTransform;
    public float distance = 10f;

    void LateUpdate()
    {
        // Đặt target phía trước camera (hướng camera nhìn)
        transform.position = cameraTransform.position + cameraTransform.forward * distance;
    }
}
