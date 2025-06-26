using UnityEngine;

public class TooltipLookAtPlayer : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;

    void Start()
    {
        // Tìm player dựa trên tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("Không tìm thấy GameObject có tag 'Player'");
        }
    }

    void LateUpdate()
    {
        if (playerTransform == null) return;

        // Lấy vị trí trên mặt phẳng (X,Z), bỏ qua trục Y để tránh nghiêng
        Vector3 direction = transform.position - playerTransform.position;
        direction.y = 0f;

        if (direction == Vector3.zero) return;

        // Quay mặt về hướng player (chỉ theo Y)
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = lookRotation;

        // Nếu bị ngược mặt thì thêm 180 độ Y
        transform.rotation = Quaternion.LookRotation(direction);

    }
}
