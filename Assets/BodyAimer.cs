using UnityEngine;

public class BodyAimer : MonoBehaviour
{
    [Header("References")]
    public Transform playerTransform;     // Gán Player
    public Transform bodyAimTarget;       // GO dùng cho IK
    public float aimDistance = 2f;        // Khoảng cách điểm aim phía trước
    public float verticalInfluence = 1f;  // Mức ảnh hưởng khi cúi/ngửa (tăng lên sẽ nghiêng mạnh hơn)
    public float verticalOffset = 1.4f;   // Cao hơn gốc player (vì player có chiều cao)

    void LateUpdate()
    {
        if (CameraTarget.Instance == null || playerTransform == null || bodyAimTarget == null)
            return;

        // Lấy pitch (chuột Y) từ CameraTarget
        float pitch = CameraTarget.Instance.GetPitch();                // Góc -30 đến +60
        float pitchRad = pitch * Mathf.Deg2Rad;                        // Đổi sang radian

        // Hướng forward player (luôn nằm trước mặt player)
        Vector3 forward = playerTransform.forward;
        forward.y = 0;
        forward.Normalize();

        // Tính offset cao/thấp theo pitch camera
        float verticalAimOffset = Mathf.Sin(pitchRad) * verticalInfluence;

        // Tính vị trí target
        Vector3 aimPos = playerTransform.position + Vector3.up * verticalOffset + forward * aimDistance;
        aimPos.y += verticalAimOffset;

        // Gán vị trí
        bodyAimTarget.position = aimPos;

        // Cho target nhìn về phía player (nếu cần cho IK)
        bodyAimTarget.LookAt(playerTransform.position + Vector3.up * verticalOffset);
    }
}
