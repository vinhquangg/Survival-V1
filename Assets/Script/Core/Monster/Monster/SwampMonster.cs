using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SwampMonster : BaseMonster
{
    public float patrolZoneRadius = 10f;
    public int patrolPointCount = 6;
    public List<Vector3> patrolPoints = new List<Vector3>();
    private int currentPatrolIndex = 0;

    protected override void Start()
    {
        base.Start();

        patrolType = PatrolType.Random;

        GeneratePatrolPoints();
    }

    public override void SetRandomPatrolDestination(float patrolRadius)
    {
        if(patrolType != PatrolType.Random) return;

        if (patrolPoints.Count == 0)
        {
            Debug.LogWarning("SwampMonster: Không có điểm tuần tra.");
            return;
        }

        _navMeshAgent.SetDestination(patrolPoints[currentPatrolIndex]);
        Debug.Log("SwampMonster patrol to point: " + patrolPoints[currentPatrolIndex]);

        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
    }

    private void GeneratePatrolPoints()
    {
        patrolPoints.Clear();
        int attempts = 0;

        while (patrolPoints.Count < patrolPointCount && attempts < patrolPointCount * 5)
        {
            Vector3 randomOffset = Random.insideUnitSphere * patrolZoneRadius;
            randomOffset.y = 0;
            Vector3 point = transform.position + randomOffset;

            if (NavMesh.SamplePosition(point, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                patrolPoints.Add(hit.position);
            }

            attempts++;
        }

        if (patrolPoints.Count == 0)
        {
            Debug.LogWarning("SwampMonster: Không tìm được điểm tuần tra nào!");
        }
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage); // vẫn giảm máu, gọi OnHealthChanged, kiểm tra chết

        // Chỉ phát âm thanh nếu chưa chết
        if (currentHeal > 0)
        {
            SoundManager.Instance.PlaySFX(SoundManager.Instance.swampHitSound);
        }
    }

    protected override void Die()
    {
        // Phát âm thanh chết
        SoundManager.Instance.PlaySFX(SoundManager.Instance.swampDeadSound);

        base.Die();
    }


    //private void OnTriggerEnter(Collider other)
    //{

    //    if (other.CompareTag("PlayerWeapon"))
    //    {
    //        float damage = 10f;
    //        currentHeal -= damage;
    //        Debug.Log($"Máu còn lại của quái: {currentHeal}");

    //        if (currentHeal <= 0)
    //        {
    //            Die();
    //        }
    //    }
    //}


}
