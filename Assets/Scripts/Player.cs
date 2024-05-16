using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    [Header("For skill")]
    public GameObject[] skills;
    public bool[] hasSkills;
    bool sDown1;
    bool sDown2;
    bool sDown3;
    bool sDown4;
    // bool sDown5;
    bool isSwap;
    int equipedIndex = -1;
    [Header("Others")]
    public float speed;
    NavMeshAgent agent;
    Animator anim;
    public Transform spot;
    LineRenderer lr;
    Coroutine draw;
    GameObject nearObject;
    GameObject equiped;
    // Start is called before the first frame update
    void Start()
    {

    }

    void GetInput(){
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
        sDown4 = Input.GetButtonDown("Swap4");
        // sDown3 = Input.GetButtonDown("Swap5");
    }

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        lr = GetComponent<LineRenderer>();
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.material.color = Color.green;
        lr.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Swap();
        ClickMove();
        Interat();

    }

    void ClickMove(){
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                agent.SetDestination(hit.point);
                anim.SetBool("isRun", true);

                spot.gameObject.SetActive(true);
                spot.position = hit.point;
                if (draw != null) StopCoroutine(draw);
                draw = StartCoroutine(DrawPath());
            }
        }
        // 애니메이션 상태를 업데이트하는 코드
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                anim.SetBool("isRun", false);
                spot.gameObject.SetActive(false);

                lr.enabled = false;

                // if(draw != null) StopCoroutine(draw);

            }
        }
    }

    IEnumerator DrawPath()
    {
        lr.enabled = true;
        yield return null;
        while (true)
        {
            int cnt = agent.path.corners.Length;
            lr.positionCount = cnt;
            for (int i = 0; i < cnt; i++)
            {
                lr.SetPosition(i, agent.path.corners[i]);
            }
            yield return null;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Skill"))
        {
            nearObject = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Skill"))
        {
            nearObject = null;
        }
    }

    void SwapOut(){
        isSwap = false;
    }

    void Swap(){

        if(sDown1 && (!hasSkills[0] || equipedIndex == 0)) return;
        if(sDown2 && (!hasSkills[1] || equipedIndex == 1)) return;
        if(sDown3 && (!hasSkills[2] || equipedIndex == 2)) return;
        if(sDown4 && (!hasSkills[3] || equipedIndex == 3)) return;
        // if(sDown1 && (!hasSkills[4] || equipedIndex == 4)) return;
        int skillIndex = -1;
        if(sDown1) skillIndex = 0;
        if(sDown2) skillIndex = 1;
        if(sDown3) skillIndex = 2;
        if(sDown4) skillIndex = 3;
        // if(sDown5) skillIndex = 4;
        
        if(sDown1 || sDown2 || sDown3 || sDown4){
            if(equiped != null) equiped.SetActive(false);

            equipedIndex = skillIndex;
            equiped = skills[skillIndex];
            skills[skillIndex].SetActive(true);

            anim.SetTrigger("doSwap");

            isSwap = true;

            Invoke("SwapOut", 0.4f);
        }
    }
    void Interat()
    {
        if (nearObject != null && nearObject.CompareTag("Skill"))
        {
            Item item = nearObject.GetComponent<Item>();
            if (item != null && item.type == Item.Type.Skill)
            {
                int skillIndex = item.index;
                hasSkills[skillIndex] = true;

                Destroy(nearObject);
            }
        }

    }
}
