using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    [Header("References")]
    public Transform target;          
    public Transform lookTarget;      

    [Header("Camera Settings")]
    public float distance = 6f;       
    public float height = 2f;         
    public float smoothSpeed = 5f;
    public float pitchSpeed = 80f;

    [Header("Clamp Settings")]
    public Vector2 pitchClamp = new Vector2(-30f, 45f);     

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
        pitch = 10f;  
    }

    private void LateUpdate()
    {
        if (target == null || lookTarget == null) return;

        if (allowCameraInput)
        {
            float mouseY = Input.GetAxis("Mouse Y");
            pitch -= mouseY * pitchSpeed * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, pitchClamp.x, pitchClamp.y);
        }

        Vector3 backward = -target.forward * distance + Vector3.up * height;
        Vector3 desiredPosition = target.position + backward;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        Quaternion rotation = Quaternion.Euler(pitch, target.eulerAngles.y, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, smoothSpeed * Time.deltaTime);

        transform.LookAt(lookTarget.position);
    }

    public float GetPitch()
    {
        return pitch;
    }
}
