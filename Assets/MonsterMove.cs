using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterMove : MonoBehaviour
{
    public Transform _destination;
    public NavMeshAgent _navMeshAgent;
    void Start()
    {
        _navMeshAgent=this.GetComponent<NavMeshAgent>();

        if(_navMeshAgent== null)
        {
            Debug.Log("Missing Navmesh");
        }
        //else
        //{
        //    SetDestination();
        //}
    }

    private void SetDestination()
    {
        if(_destination !=null)
        {
            Vector3 targetVector = _destination.transform.position;
            _navMeshAgent.SetDestination(targetVector);
        }
    }

    // Update is called once per frame
    void Update()
    {
        SetDestination();
    }
}
