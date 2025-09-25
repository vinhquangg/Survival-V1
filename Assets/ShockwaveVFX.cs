using UnityEngine;

public class ShockwaveVFX : MonoBehaviour
{
    public float expandSpeed = 5f;
    public float lifeTime = 1.5f;
    public int damage;
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        // scale dome lan ra
        transform.localScale += Vector3.one * expandSpeed * Time.deltaTime;

        // hủy sau lifetime
        if (timer >= lifeTime)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<IDamageable>(out var dmg))
            {
                dmg.TakeDamage(damage);
                Debug.Log("Shockwave hit player!");
            }
        }
    }
}
