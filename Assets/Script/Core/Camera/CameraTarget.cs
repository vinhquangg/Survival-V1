using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    [Header("References")]
    public Transform target;          // → gán Transform player
    public Transform lookTarget;      // → gán Transform điểm giữa ngực hoặc đầu player

    [Header("Camera Settings")]
    public float distance = 6f;            // Khoảng cách sau lưng player
    public float height = 2f;              // Độ cao camera
    public float smoothSpeed = 5f;
    public float pitchSpeed = 80f;

    [Header("Clamp Settings")]
    public Vector2 pitchClamp = new Vector2(-30f, 45f);     // Giới hạn cúi/ngửa người

    [Header("Control")]
    public bool allowCameraInput = true;

    public static CameraTarget Instance { get; private set; }

    private float pitch = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        pitch = 10f;  // mặc định hơi nhìn xuống
    }

    private void LateUpdate()
    {
        if (target == null || lookTarget == null) return;

        // Lấy input xoay camera lên xuống
        if (allowCameraInput)
        {
            float mouseY = Input.GetAxis("Mouse Y");
            pitch -= mouseY * pitchSpeed * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, pitchClamp.x, pitchClamp.y);
        }

        // Tính vị trí phía sau lưng player theo hướng forward
        Vector3 backward = -target.forward * distance + Vector3.up * height;
        Vector3 desiredPosition = target.position + backward;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Tính góc nhìn lên/xuống từ camera → lookTarget
        Quaternion rotation = Quaternion.Euler(pitch, target.eulerAngles.y, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, smoothSpeed * Time.deltaTime);

        // Camera luôn nhìn vào điểm trên thân player
        transform.LookAt(lookTarget.position);
    }

    public float GetPitch()
    {
        return pitch;
    }
}
