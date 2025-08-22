using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    public Transform target;
    //public Vector3 offset;

    void LateUpdate()
    {
        Vector3 newPos= target.position;

        newPos.y = transform.position.y; 
        transform.position = newPos;
    }
}
