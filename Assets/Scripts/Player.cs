using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    public float speed;
    NavMeshAgent agent;
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake(){
        anim = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out RaycastHit hit)){
                agent.SetDestination(hit.point);
                anim.SetBool("isRun", true);
            }
        }
         // 애니메이션 상태를 업데이트하는 코드
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                anim.SetBool("isRun", false);
            }
        }
        
    }
}
