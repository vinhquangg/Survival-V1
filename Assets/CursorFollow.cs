using UnityEngine;

public class CursorFollower : MonoBehaviour
{
    public Transform playerTransform;   // → gán Transform thân player
    public Transform cameraTransform;   // → gán Main Camera
    public float distance = 2.5f;

    void Update()
    {
        // lấy góc xoay
        float pitch = cameraTransform.eulerAngles.x;    // xoay lên xuống từ camera
        float yaw = playerTransform.eulerAngles.y;       // xoay trái/phải từ player

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        transform.position = playerTransform.position + rotation * Vector3.forward * distance;
        transform.rotation = rotation;
    }
}
