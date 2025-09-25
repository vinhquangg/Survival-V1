using UnityEngine;

public class MonsterAttackState : MonsterBaseState
{
    private float attackCooldown = 1.5f;
    private float cooldownTimer = 0f;
    private bool isAttacking = false;
    private bool isBite = false;

    private bool isCast = false;  
    private float castCooldown = 10f;
    private float castTimer = 0f;

    public MonsterAttackState(MonsterStateMachine stateMachine) : base(stateMachine) { }
    public override void EnterState()
    {
        monster._navMeshAgent.ResetPath(); // Dừng di chuyển
        cooldownTimer = 0f;
        isAttacking = true;
        isCast = false;
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
            // Luôn quay về player khi đang attack/bite
            if (isAttacking || isBite)
                monster.combat.RotateTowardsTarget(monster.combat.rotationSpeed);

            castTimer -= Time.deltaTime;
            if (ranged.IsBoss && !isCast && castTimer <= 0f)
            {
                // Boss sẽ cast spell (shockwave)
                isCast = true;
                castTimer = castCooldown;

                stateMachine.animator.SetTrigger("isCast"); // trigger anim cast spell
                return; // ưu tiên cast spell, không attack thông thường
            }

            // --- ƯU TIÊN ATTACK XA ---
            cooldownTimer += Time.deltaTime;
            if (!isAttacking && cooldownTimer >= attackCooldown)
            {
                isAttacking = true;
                cooldownTimer = 0f;
                stateMachine.animator.SetTrigger("isAttack"); // Ưu tiên bắn projectile
                return;
            }

            // --- CHỈ BITE KHI PLAYER QUÁ GẦN ---
            if (ranged.CanBite() && !isBite && !isAttacking)
            {
                isBite = true;
                cooldownTimer = 0f;
                stateMachine.animator.SetTrigger("isBite");
                return;
            }

            return;
        }

        // --- MELEE MONSTER ---
        if (monster.combat is MeleeMonsterCombat)
        {
            if (isAttacking)
                monster.combat.RotateTowardsTarget(monster.combat.rotationSpeed);

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
            else
            {
                //// Nếu đang đánh mà player chạy ra xa → hủy attack
                //if (isAttacking)
                //{
                //    stateMachine.animator.ResetTrigger("isAttack");
                //    isAttacking = false;
                //}

                // Chuyển sang chase
                if (!isAttacking)
                {
                    stateMachine.SwitchState(new MonsterChaseState(stateMachine));
                    return;
                }
            }


            return;
        }

        if (monster.combat is BearCombat bear)
        {
            if (isAttacking)
                monster.combat.RotateTowardsTarget(monster.combat.rotationSpeed);

            if (distanceToPlayer <= monster.combat.attackRange)
            {
                if (!isAttacking)
                {
                    isAttacking = true;
                    cooldownTimer = 0f;

                    // Ưu tiên chọn skill theo khoảng cách
                    if (bear.CanBite())
                    {
                        isBite = true;
                        stateMachine.animator.SetTrigger("isAttack");
                        Debug.Log("Bear: BITE attack");
                    }
                    else if (bear.CanClaw())
                    {

                        stateMachine.animator.SetTrigger("isClaw");
                        Debug.Log("Bear: CLAW attack");
                    }
                    else
                    {
                        // Nếu không đủ gần → chase tiếp
                        isAttacking = false;
                        stateMachine.SwitchState(new MonsterChaseState(stateMachine));
                        return;
                    }
                }
            }
            else if (!isAttacking)
            {
                // Player quá xa → chase
                stateMachine.SwitchState(new MonsterChaseState(stateMachine));
                return;
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
        isCast = false;
    }
}
