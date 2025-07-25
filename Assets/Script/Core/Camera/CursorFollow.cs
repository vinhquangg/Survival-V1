using UnityEngine;

public class CursorFollower : MonoBehaviour
{
    public Transform cameraTransform;
    public float distance = 2.5f;

    void Update()
    {
        transform.position = cameraTransform.position + cameraTransform.forward * distance;
        transform.rotation = cameraTransform.rotation;
    }
}
