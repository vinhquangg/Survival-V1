using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody rb;
    private int arrowDamage = 25;

    
    private bool isStuck = false; // Flag to track if the arrow is stuck
    private bool hasDealtDamage = false;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        Destroy(gameObject, 5f); // Destroy the arrow after 5 seconds if it doesn't hit anything

        Collider arrowCollider = GetComponent<Collider>();
    }

    // This method is called when the arrow hits a collider
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Arrow collided with: " + collision.transform.name);
        // Check if the arrow is not already stuck
        if (!isStuck && !collision.transform.CompareTag("Player"))
        {
            isStuck = true; // Mark the arrow as stuck

            // Stop the movement by setting the Rigidbody's velocity and angular velocity to zero
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Optionally, freeze the Rigidbody's movement and rotation completely
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
