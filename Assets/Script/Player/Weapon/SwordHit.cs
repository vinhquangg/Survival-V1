using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordHit : MonoBehaviour
{
    public float damage = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<BaseMonster>(out var monster))
        {
            Debug.Log($"Đánh trúng: {monster.name}");
            monster.TakeDamage(damage);
        }
    }
}
