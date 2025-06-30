using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    public Transform target;          
    public Transform lookTarget;      
    public Vector3 offset = new Vector3(0f, 5f, -10f);
    public float smoothSpeed = 5f;
    public float pitchSpeed = 80f;
    public Vector2 pitchClamp = new Vector2(-30f, 60f);
    public static CameraTarget Instance { get; private set; }
    private float yaw = 0f; 

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

    void LateUpdate()
    {
        if (target == null || lookTarget == null) return;

        float mouseY = Input.GetAxis("Mouse Y");
        pitch -= mouseY * pitchSpeed * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, pitchClamp.x, pitchClamp.y);

        yaw = target.eulerAngles.y;

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desiredPosition = target.position + rotation * offset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.LookAt(lookTarget.position);
    }

}
