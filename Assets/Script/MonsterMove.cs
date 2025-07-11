//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.AI;

//public class MonsterMove : MonoBehaviour
//{
//    public Transform player;
//    public NavMeshAgent _navMeshAgent;
//    public float DetectedRange;
//    void Start()
//    {
//        _navMeshAgent=this.GetComponent<NavMeshAgent>();
//        player = GameObject.FindWithTag("Player").transform;
//        if (_navMeshAgent== null)
//        {
//            Debug.Log("Missing Navmesh");
//        }
//        //else
//        //{
//        //    SetDestination();
//        //}
//    }

//    //private bool CanSeePlayer()
//    //{
//    //    if (player == null) return false;
//    //    Vector3 directionToPlayer = player.position - transform.position;
//    //    if (directionToPlayer.magnitude < DetectedRange) return true;
//    //    return false;
//    //}

//    private bool CanSeePlayer()
//    {
//        if (player == null) return false;

//        Vector3 directionToPlayer = player.position - transform.position;
//        float distanceToPlayer = directionToPlayer.magnitude;

//        if (distanceToPlayer < DetectedRange)
//        {
//            Ray ray = new Ray(transform.position + Vector3.up, directionToPlayer.normalized); 
//            RaycastHit hit;

//            if (Physics.Raycast(ray, out hit, DetectedRange))
//            {
//                if (hit.transform == player)
//                {
//                    return true;
//                }
//            }
//        }

//        return false;
//    }


//    private void SetDestination()
//    {
//        if(player !=null)
//        {
//            Vector3 targetVector = player.transform.position;
//            _navMeshAgent.SetDestination(targetVector);
//        }
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (CanSeePlayer())
//        {
//            SetDestination();
//        }
//    }
//}
