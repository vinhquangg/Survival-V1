using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    public Transform target;
    //public Vector3 offset;
    private void Start()
    {
        if (target == null && PlayerManager.Instance != null)
        {
            var player = PlayerManager.Instance.GetCurrentPlayer();
            if (player != null)
                target = player.transform;
        }
    }
    void LateUpdate()
    {
        Vector3 newPos= target.position;

        newPos.y = transform.position.y; 
        transform.position = newPos;
    }
}
