using UnityEngine;

public class MonsterAttackState : IInterfaceMonsterState
{
    private MonsterStateMachine enemy;
    private float attackCooldown = 1.5f;
    private float cooldownTimer = 0f;
    private bool isAttacking = false;

    public MonsterAttackState(MonsterStateMachine enemy)
    {
        this.enemy = enemy;
    }

    public void EnterState()
    {
        enemy.baseMonster._navMeshAgent.ResetPath(); // Dừng di chuyển
        cooldownTimer = 0f;
        isAttacking = true;
        enemy.animator.SetTrigger("isAttack"); // Trigger attack anim
    }

    public void ExitState()
    {
        isAttacking = false;
    }

    public void FixedUpdateState() { }

    public void UpdateState()
    {
        float distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.baseMonster.player.position);

        // Nếu không còn thấy player → chuyển về Idle
        if (!enemy.baseMonster.CanSeePlayer())
        {
            enemy.SwitchState(new MonsterIdleState(enemy));
            return;
        }

        // Nếu thấy nhưng không đủ gần → quay về Chase
        if (distanceToPlayer > enemy.baseMonster.combat.attackRange && !isAttacking)
        {
            enemy.SwitchState(new MonsterChaseState(enemy));
            return;
        }

        // Nếu đủ gần và thấy → thực hiện attack logic
        cooldownTimer += Time.deltaTime;
        if (!isAttacking && cooldownTimer >= attackCooldown)
        {
            isAttacking = true;
            cooldownTimer = 0f;
            enemy.animator.SetTrigger("isAttack");
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
        isAttacking = false;
    }
}
