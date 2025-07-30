using UnityEngine;

public class CursorFollower : MonoBehaviour
{

    public Transform playerTransform;   
    public Transform cameraTransform;   

    public Transform cameraTransform;

    public float distance = 2.5f;

    void Update()
    {

        // lấy góc xoay
        float pitch = cameraTransform.eulerAngles.x;    
        float yaw = playerTransform.eulerAngles.y;      

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        transform.position = playerTransform.position + rotation * Vector3.forward * distance;
        transform.rotation = rotation;

        transform.position = cameraTransform.position + cameraTransform.forward * distance;
        transform.rotation = cameraTransform.rotation;

    }
}
