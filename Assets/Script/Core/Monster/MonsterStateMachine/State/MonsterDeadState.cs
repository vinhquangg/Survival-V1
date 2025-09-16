using UnityEngine;

public class MonsterDeadState : MonsterBaseState
{
    private bool isDeadHandled = false;

    public MonsterDeadState(MonsterStateMachine stateMachine) : base(stateMachine) { }


    public override void EnterState()
    {
        Debug.Log("Monster is Dead.");
        stateMachine.animator.SetBool("isDead", true);

        if (monster._navMeshAgent != null)
        {
            monster._navMeshAgent.isStopped = true;
            monster._navMeshAgent.enabled = false; // disable hẳn
        }

        if (!isDeadHandled)
        {
            stateMachine.animator.SetBool("isChase", false);
            stateMachine.animator.SetBool("isAttack", false);
            monster.PlayAnimation(MonsterAnimState.Death);
            isDeadHandled = true;
        }

        Collider col = monster.GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }


    public override void ExitState()
    {
        // Dead thường không có ExitState, nhưng vẫn để trống cho chuẩn
    }

    public override void UpdateState()
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

    public override void FixedUpdateState()
    {
        // Dead không cần xử lý vật lý
    }
}
