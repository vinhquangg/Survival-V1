using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildBear : BaseMonster
{
    public List<Transform> waypointPatrols = new List<Transform>();
    private int currentWaypointIndex = 0;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        patrolType = PatrolType.Waypoint;

        GameObject patrolRoot = GameObject.Find("PatrolPoint");
        if (patrolRoot != null)
        {
            waypointPatrols.Clear();

            foreach (Transform child in patrolRoot.transform)
            {
                waypointPatrols.Add(child);
            }

            Debug.Log($"WildBear: Đã lấy {waypointPatrols.Count} waypoint từ PatrolPoint.");
        }
        else
        {
            Debug.LogWarning("WildBear: Không tìm thấy PatrolPoint trong scene!");
        }


    }

    public override void SetRandomPatrolDestination(float patrolRadius)
    {
        if (patrolType != PatrolType.Waypoint) return;

        if (waypointPatrols.Count == 0 || waypointPatrols == null)
        {
            Debug.LogWarning("WildBear: Không có điểm tuần tra.");
            return;
        }

        Transform nextPoint = waypointPatrols[currentWaypointIndex];

        if(nextPoint != null)
        {
            _navMeshAgent.SetDestination(nextPoint.position);
        }
        currentWaypointIndex = (currentWaypointIndex + 1) % waypointPatrols.Count;

    }

    protected override void Die()
    {
        if(_stateMachine != null)
        {
            _stateMachine.SwitchState(new MonsterDeadState(_stateMachine));
        }
        else
        {
            base.Die();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
