using UnityEngine;
using System.Collections.Generic;

public class VisibilityCullingZone : MonoBehaviour
{
    public float outerRadius = 100f;  // Vùng nhìn xa - chỉ hiện
    public float innerRadius = 50f;   // Vùng gần - cho phép tương tác

    public string tagToCheck = "Obstacle"; // Tag của object cần xét (ví dụ: cây, đá,...)

    private Transform player;
    private List<GameObject> trackedObjects = new List<GameObject>();

    private void Start()
    {
        player = this.transform; // Gán chính Camera (gắn script này vào Camera)

        // Tìm tất cả object có tag (kể cả object là con của object khác như Forest)
        GameObject[] objs = GameObject.FindGameObjectsWithTag(tagToCheck);
        trackedObjects.AddRange(objs);
    }

    private void Update()
    {
        if (player == null) return;

        foreach (GameObject obj in trackedObjects)
        {
            if (obj == null) continue;

            float distance = Vector3.Distance(player.position, obj.transform.position);

            bool inOuter = distance <= outerRadius;
            bool inInner = distance <= innerRadius;

            // Tắt/bật toàn bộ GameObject (bao gồm cả renderer, collider, script…)
            obj.SetActive(inOuter);

            // Nếu muốn collider chỉ hoạt động trong inner radius, phải bật lại thủ công sau khi SetActive(true)
            if (inOuter)
            {
                Collider[] colliders = obj.GetComponentsInChildren<Collider>(true);
                foreach (Collider col in colliders)
                    col.enabled = inInner;
            }
        }
    }



    private void OnDrawGizmosSelected()
    {
        if (player == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(player.position, outerRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(player.position, innerRadius);
    }
}
