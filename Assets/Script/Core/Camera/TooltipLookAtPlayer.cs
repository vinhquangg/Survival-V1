using UnityEngine;

public class TooltipLookAtPlayer : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;

    void Start()
    {
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

        Vector3 direction = transform.position - playerTransform.position;
        direction.y = 0f;

        if (direction == Vector3.zero) return;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = lookRotation;

        transform.rotation = Quaternion.LookRotation(direction);

    }
}
