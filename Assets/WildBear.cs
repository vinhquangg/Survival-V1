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

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage); 

        if (currentHeal > 0)
        {
            SoundManager.Instance.PlaySFX(SoundManager.Instance.animalBearHitSound);
        }
    }

    protected override void Die()
    {
        // Phát âm thanh chết
        SoundManager.Instance.PlaySFX(SoundManager.Instance.animalBearDeadSound);

        base.Die();
    }


}
