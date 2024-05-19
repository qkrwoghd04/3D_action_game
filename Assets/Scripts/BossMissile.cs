using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class BossMissile : Bullet
{
    public Transform target;
    NavMeshAgent nav;
    void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        Invoke("DestroyMissile", 5f);
    }
    void Update()
    {
        nav.SetDestination(target.position);
    }

     void DestroyMissile()
    {
        // 미사일을 파괴합니다.
        Destroy(gameObject);
    }
}   
