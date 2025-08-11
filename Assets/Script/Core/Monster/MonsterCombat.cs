using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonsterCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    public float attackRange = 2.0f; // Range within which the monster can attack
    public float attackCooldown = 1.0f; // Time between attacks
    public int attackDamage = 10; // Damage dealt by the monster's attack

    [Header("Target")]
    public Transform target; // The target the monster will attack, typically the player
    public Animator animator;

    protected float lastAttackTime = 0f; // Time when the monster last attacked 

    public bool CanAttack()
    {
        if (target == null) return false;

        // Check if the target is within attack range
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        return Time.time >= lastAttackTime && distanceToTarget <= attackRange;
    }

    public void TryAttack()
    {
        if (CanAttack())
        {
            Attack();
            LastAttackTime(); // Update the last attack time after a successful attack
        }
    }
    protected void LastAttackTime()
    {
        lastAttackTime = Time.time;
    }

    protected abstract void Attack(); // Abstract method to be implemented by derived classes for specific attack behavior
}
