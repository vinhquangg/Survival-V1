using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody rb;
    private int arrowDamage = 25;

    private bool isStuck = false;
    private bool hasDealtDamage = false;

    private GameObject owner; // người bắn

    public void SetOwner(GameObject shooter)
    {
        owner = shooter;

        // Ignore tất cả collider của owner
        Collider arrowCol = GetComponent<Collider>();
        if (arrowCol != null && owner != null)
        {
            foreach (Collider col in owner.GetComponentsInChildren<Collider>())
            {
                Physics.IgnoreCollision(arrowCol, col);
            }
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, 5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Arrow collided with: " + collision.transform.name);

        if (!isStuck && (owner == null || collision.gameObject != owner))
        {
            isStuck = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;

            Debug.Log("Arrow stuck! :" + collision.transform.name);
        }

        if (!hasDealtDamage && collision.transform.GetComponentInParent<IDamageable>() is IDamageable damageable)
        {
            hasDealtDamage = true;
            damageable.TakeDamage(arrowDamage);
        }
    }
}
