using UnityEngine;

public class MonsterDeadState : MonsterBaseState
{
    private bool isDeadHandled = false;

    public MonsterDeadState(MonsterStateMachine stateMachine) : base(stateMachine) { }


    public override void EnterState()
    {
        //if (monster is DragonMonster dragon)
        //{
        //    if (dragon.isBoss)
        //        SoundManager.Instance?.PlaySFX(SoundManager.Instance.dragonDeadSound);
        //    else
        //        SoundManager.Instance?.PlaySFX(SoundManager.Instance.dragonHitSound);
        //}
        //else if (monster is WildBear bear)
        //{
        //    if (SoundManager.Instance != null)
        //        SoundManager.Instance.PlaySFX(SoundManager.Instance.animalBearDeadSound);
        //}
        //else
        //{
        //    if (SoundManager.Instance != null)
        //        SoundManager.Instance.PlaySFX(SoundManager.Instance.swampDeadSound);
        //}
        stateMachine.animator.SetBool("isDead", true);

        if (monster._navMeshAgent != null && monster._navMeshAgent.isOnNavMesh)
        {
            monster._navMeshAgent.isStopped = true;
            monster._navMeshAgent.enabled = false; // disable hẳn
        }

        if (!isDeadHandled)
        {
            stateMachine.animator.SetBool("isChase", false);
            stateMachine.animator.SetBool("isAttack", false);
            monster.PlayAnimation(MonsterAnimState.Death);
            monster.DropItems();
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
        //monster.healthUIPrefab.SetActive(false);
        // Sau khi anim Die xong thì xử lý logic
        //if (!isDeadHandled && enemy.animator.GetCurrentAnimatorStateInfo(0).IsName("Die_Monster"))
        //{
        //if (stateMachine.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
        //{
        //    isDeadHandled = true;
        //    monster.ReturnPool();
        //monster.gameObject.SetActive(false);

        // Xử lý drop item
        //monster.DropItems();

        //// Ẩn quái hoặc trả về pool

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
