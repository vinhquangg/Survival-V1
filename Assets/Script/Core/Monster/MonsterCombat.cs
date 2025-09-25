using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonsterCombat : MonoBehaviour
{
    public float attackRange { get; protected set; }
    public float attackCooldown { get; protected set; }
    public int attackDamage { get; protected set; }

    [Header("Target")]
    public Transform target; // The target the monster will attack, typically the player
    public Animator animator;
    protected BaseMonster monster;
    public float rotationSpeed { get; protected set; } = 5f;
    protected float lastAttackTime = 0f; // Time when the monster last attacked 

    protected virtual void Start()
    {
        monster = GetComponent<BaseMonster>();
    }

    public virtual void SetupFromStats(MonsterStatsSO stats)
    {
        attackRange = stats.attackRange;
        attackCooldown = stats.attackCooldown;
        attackDamage = stats.attackDamage;
    }

    public bool CanAttack()
    {
        if (target == null) return false;

        // Check if the target is within attack range
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        return Time.time >= lastAttackTime && distanceToTarget <= attackRange;
    }

    public void RotateTowardsTarget(float rotationSpeed)
    {
        if (target == null) return;

        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0; // Giữ nguyên chiều cao, tránh xoay đầu lên/xuống

        if (direction.magnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    public void TryAttack()
    {
        if (CanAttack())
        {
            //RotateTowardsTarget(rotationSpeed);
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
