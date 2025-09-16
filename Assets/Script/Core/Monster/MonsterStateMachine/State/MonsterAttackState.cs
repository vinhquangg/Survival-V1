using UnityEngine;

public class MonsterAttackState : MonsterBaseState
{
    private float attackCooldown = 1.5f;
    private float cooldownTimer = 0f;
    private bool isAttacking = false;
    private bool isBite = false;

    public MonsterAttackState(MonsterStateMachine stateMachine) : base(stateMachine) { }
    public override void EnterState()
    {
        monster._navMeshAgent.ResetPath(); // Dừng di chuyển
        cooldownTimer = 0f;
        isAttacking = true;
        stateMachine.animator.SetTrigger("isAttack"); // Trigger attack anim
    }

    public override void ExitState()
    {
        isAttacking = false;
    }

    public override void FixedUpdateState() { }

    public override void UpdateState()
    {
        if (monster.player == null) return;

        float distanceToPlayer = Vector3.Distance(stateMachine.transform.position, monster.player.position);

        // Nếu không thấy player → về Idle
        if (!monster.CanSeePlayer())
        {
            stateMachine.SwitchState(new MonsterIdleState(stateMachine));
            return;
        }

        // --- RANGE MONSTER ---
        if (monster.combat is RangeMonsterCombat ranged)
        {
            // Luôn quay về player nếu đang attack/bite
            if (isBite || isAttacking)
                monster.combat.RotateTowardsTarget(monster.combat.rotationSpeed);

            // Player gần → Bite
            if (ranged.CanBite())
            {
                if (!isBite)
                {
                    isBite = true;
                    isAttacking = false;
                    cooldownTimer = 0f;
                    //monster.combat.RotateTowardsTarget(monster.combat.rotationSpeed);
                    stateMachine.animator.SetTrigger("isBite");
                }
                return; // ưu tiên cắn → không bắn projectile
            }

            // Player xa nhưng trong attack range → ShootProjectile
            cooldownTimer += Time.deltaTime;
            if (!isAttacking && cooldownTimer >= attackCooldown)
            {
                isAttacking = true;
                cooldownTimer = 0f;
                //monster.combat.RotateTowardsTarget(monster.combat.rotationSpeed);
                stateMachine.animator.SetTrigger("isAttack");
            }

            return;
        }

        // --- MELEE MONSTER ---
        if (monster.combat is MeleeMonsterCombat)
        {
            if (distanceToPlayer <= monster.combat.attackRange)
            {
                // Attack khi gần
                if (!isAttacking)
                {
                    isAttacking = true;
                    cooldownTimer = 0f;
                    //monster.combat.RotateTowardsTarget(monster.combat.rotationSpeed);
                    stateMachine.animator.SetTrigger("isAttack");
                }
                else
                {
                    // Luôn quay về player khi đang attack
                    //monster.combat.RotateTowardsTarget(monster.combat.rotationSpeed);
                }
            }
            else if (!isAttacking)
            {
                // Quá xa → Chase
                stateMachine.SwitchState(new MonsterChaseState(stateMachine));
            }

            return;
        }
    }




    //public void UpdateState()
    //{

    //    float distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.baseMonster.player.position);

    //    if (distanceToPlayer > enemy.baseMonster.combat.attackRange && !isAttacking)
    //    {
    //        enemy.SwitchState(new MonsterChaseState(enemy));
    //        return;
    //    }

    //    enemy.baseMonster.CanSeePlayer();

    //    // Tính cooldown và trigger anim mới nếu chưa tấn công
    //    cooldownTimer += Time.deltaTime;
    //    if (!isAttacking && cooldownTimer >= attackCooldown)
    //    {
    //        isAttacking = true;
    //        cooldownTimer = 0f;
    //        enemy.animator.SetTrigger("isAttack");
    //    }
    //}

    // Gọi từ Animation Event khi animation kết thúc
    public void OnAttackAnimationFinished()
    {
        isBite = false;
        isAttacking = false;
    }
}
