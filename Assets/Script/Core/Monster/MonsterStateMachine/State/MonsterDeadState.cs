using UnityEngine;

public class MonsterDeadState : IInterfaceMonsterState
{
    private MonsterStateMachine enemy;
    private bool isDeadHandled = false;

    public MonsterDeadState(MonsterStateMachine enemy)
    {
        this.enemy = enemy;
    }

    public void EnterState()
    {
        Debug.Log("Monster is Dead.");
        enemy.animator.SetBool("isDead", true);

        if (enemy.baseMonster._navMeshAgent != null)
        {
            enemy.baseMonster._navMeshAgent.isStopped = true;
            enemy.baseMonster._navMeshAgent.enabled = false; // disable hẳn
        }

        if (!isDeadHandled)
        {
            enemy.animator.SetBool("isChase", false);
            enemy.animator.SetBool("isAttack", false);
            enemy.baseMonster.PlayAnimation(MonsterAnimState.Death);
            isDeadHandled = true;
        }

        Collider col = enemy.baseMonster.GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }


    public void ExitState()
    {
        // Dead thường không có ExitState, nhưng vẫn để trống cho chuẩn
    }

    public void UpdateState()
    {
        // Sau khi anim Die xong thì xử lý logic
        //if (!isDeadHandled && enemy.animator.GetCurrentAnimatorStateInfo(0).IsName("Die_Monster"))
        //{
        //    if (enemy.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
        //    {
        //        isDeadHandled = true;

        //        //// Xử lý drop item
        //        //enemy.baseMonster.DropItems();

        //        //// Ẩn quái hoặc trả về pool
        //        //if (enemy.baseMonster.usePooling)
        //        //{
        //        //    enemy.baseMonster.ReturnToPool();
        //        //}
        //        //else
        //        //{
        //        //    GameObject.Destroy(enemy.gameObject, 2f);
        //        //}
        //    }
        //}
    }

    public void FixedUpdateState()
    {
        // Dead không cần xử lý vật lý
    }
}
