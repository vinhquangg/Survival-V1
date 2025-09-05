using UnityEngine;

public class CursorFollow : MonoBehaviour
{
    public Transform playerTransform;   
    public Transform cameraTransform;   
    public float distance = 2.5f;
    private void Start()
    {
        if (playerTransform == null && PlayerManager.Instance != null)
        {
            var player = PlayerManager.Instance.GetCurrentPlayer();
            if (player != null)
                playerTransform = player.transform;
        }

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
            Debug.Log("CameraTransform đã gán = " + cameraTransform.name);
        }
    }
    void Update()
    {
        // lấy hướng ngang (yaw) từ player
        Quaternion yawRotation = Quaternion.Euler(0f, playerTransform.eulerAngles.y, 0f);

        // lấy hướng dọc (pitch) từ camera
        Quaternion pitchRotation = Quaternion.Euler(cameraTransform.eulerAngles.x, 0f, 0f);

        // combine lại
        Quaternion rotation = yawRotation * pitchRotation;

        // vị trí cursor = trước mặt player, xoay theo rotation
        transform.position = playerTransform.position + rotation * Vector3.forward * distance;
        transform.rotation = rotation;
    }

}
