using UnityEngine;

public enum MonsterType
{
    Melee,
    Ranged
}

[CreateAssetMenu(fileName = "MonsterStats", menuName = "SO/Monster Stats")]
public class MonsterStatsSO : ScriptableObject
{
    [Header("General Stats")]
    public MonsterType monsterType;
    public string monsterName;
    public float maxHealth = 100f;
    public float moveSpeed = 3.5f;
    public float patrolSpeed = 1.5f;
    public float detectedRange = 10f;
    public float hitDuration;
    public float graceTimer = 0f;
    public float graceTimeMax = 0.5f;

    [Header("Combat Stats")]
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public int attackDamage = 10;

    [Header("Knockback Settings")]
    public bool canKnockback = true;
    public float knockbackForce = 5f;
}
