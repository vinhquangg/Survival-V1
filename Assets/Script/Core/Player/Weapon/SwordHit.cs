//using UnityEngine;

//public class SwordHit : MonoBehaviour
//{
//    public float damage = 10f;
//    public bool isAttacking = false; // ✅ Thêm flag

//    private void OnTriggerEnter(Collider other)
//    {
//        if (!isAttacking) return; // ❌ Nếu chưa tấn công thì không làm gì

//        if (other.TryGetComponent<IDamageable>(out var damageable))
//        {
//            damageable.TakeDamage(damage);
//        }
//    }

//    // 📌 Gọi từ animation event
//    public void EnableAttack()
//    {
//        isAttacking = true;
//    }

//    public void DisableAttack()
//    {
//        isAttacking = false;
//    }
//}
