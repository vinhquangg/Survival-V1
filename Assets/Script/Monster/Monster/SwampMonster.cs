using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SwampMonster : BaseMonster
{
    public float patrolZoneRadius = 10f;
    public int patrolPointCount = 6;
    public List<Vector3> patrolPoints = new List<Vector3>();
    private int currentPatrolIndex = 0;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        GeneratePatrolPoints();
    }


    public override bool CanSeePlayer()
    {
        return base.CanSeePlayer();
    }

    public override void SetDestination()
    {
        base.SetDestination();
    }

    public override void SetRandomPatrolDestination(float unused)
    {
        if (patrolPoints.Count == 0) return;

        _navMeshAgent.SetDestination(patrolPoints[currentPatrolIndex]);
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;

        Debug.Log("SwampMonster patrol to point: " + patrolPoints[currentPatrolIndex]);
    }


    private void GeneratePatrolPoints()
    {
        patrolPoints.Clear();
        int attempts = 0;

        while (patrolPoints.Count < patrolPointCount && attempts < patrolPointCount * 5)
        {
            Vector3 randomOffset = Random.insideUnitSphere * patrolZoneRadius;
            randomOffset.y = 0; // giữ mặt đất
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

}
