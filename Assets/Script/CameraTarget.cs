using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    public Transform target;          
    public Transform lookTarget;      
    public Vector3 offset = new Vector3(0f, 5f, -10f);
    public float smoothSpeed = 5f;
    public float pitchSpeed = 80f;
    public Vector2 pitchClamp = new Vector2(-30f, 60f);
    private float yaw = 0f; // thêm để xoay theo player

    private float pitch = 0f;

    void LateUpdate()
    {
        if (target == null || lookTarget == null) return;

        // xử lý pitch (xoay lên/xuống)
        float mouseY = Input.GetAxis("Mouse Y");
        pitch -= mouseY * pitchSpeed * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, pitchClamp.x, pitchClamp.y);

        // xoay ngang (yaw) theo hướng Player
        yaw = target.eulerAngles.y;

        // tạo quaternion từ pitch (X) và yaw (Y)
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desiredPosition = target.position + rotation * offset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.LookAt(lookTarget.position);
    }

}
