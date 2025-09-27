using Pinwheel.Poseidon;

using UnityEngine;

public class FloatBoat : MonoBehaviour
{
    [Header("Water")]
    public PWater water;                // gán water object Poseidon trong scene
    public float buoyancyForce = 15f;    // lực nổi
    public float waterDrag = 1f;
    public float waterAngularDrag = 1f;

    [Header("Buoyancy Points")]
    public Transform[] floatPoints;      // các điểm trên đáy thuyền
    [Header("Stabilization")]
    public float stabilityTorque = 10f; //
    private Rigidbody rb;
    private float defaultDrag;
    private float defaultAngularDrag;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        defaultDrag = rb.drag;
        defaultAngularDrag = rb.angularDrag;
    }

    void FixedUpdate()
    {
        if (water == null || floatPoints.Length == 0) return;

        int underWater = 0;
        float waterLevel = water.transform.position.y; // Poseidon gốc là phẳng

        foreach (Transform p in floatPoints)
        {
            float diff = p.position.y - waterLevel;

            if (diff < 0f) // điểm chìm dưới mặt nước
            {
                // Buoyancy cơ bản
                rb.AddForceAtPosition(Vector3.up * buoyancyForce * Mathf.Abs(diff),
                                      p.position,
                                      ForceMode.Force);

                // --- THÊM LỰC DAO ĐỘNG LÊN XUỐNG ---
                float waveBobbing = Mathf.Sin(Time.time * 2.5f + p.position.x + p.position.z) * 2.5f;
                rb.AddForceAtPosition(Vector3.up * waveBobbing, p.position, ForceMode.Force);
                // Giá trị 2f ở trên là biên độ dao động (có thể chỉnh)
                // Time.time * 2f là tần số (có thể tăng/giảm)
                // + p.position.x/z để các điểm float dao động lệch pha, cảm giác thuyền nghiêng tự nhiên

                underWater++;
            }
        }

        if (underWater > 0)
        {
            rb.drag = waterDrag;
            rb.angularDrag = waterAngularDrag;
        }
        else
        {
            rb.drag = defaultDrag;
            rb.angularDrag = defaultAngularDrag;
        }

        // --- Thêm ổn định thuyền ---
        Vector3 uprightTorque = Vector3.Cross(transform.up, Vector3.up) * stabilityTorque;
        rb.AddTorque(uprightTorque, ForceMode.Force);
    }


    private void OnDrawGizmos()
    {
        if (floatPoints == null) return;
        Gizmos.color = Color.cyan;
        foreach (var p in floatPoints)
        {
            if (p != null) Gizmos.DrawSphere(p.position, 0.1f);
        }
    }
}
