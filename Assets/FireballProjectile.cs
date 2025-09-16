using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    private float lifeTime = 5f;
    private float timer;

    private void OnEnable()
    {
        timer = 0f;
    }

    // ✨ Gọi khi spawn projectile
    public void Launch(Vector3 dir, float projectileSpeed)
    {
        direction = dir.normalized;
        speed = projectileSpeed;
        transform.forward = direction; // đảm bảo projectile quay đúng hướng
        timer = 0f;
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        timer += Time.deltaTime;
        if (timer >= lifeTime)
            gameObject.SetActive(false); // return về pool
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Environment"))
        {
            var damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
                damageable.TakeDamage(20);

            gameObject.SetActive(false); // return về pool
        }
    }
}
