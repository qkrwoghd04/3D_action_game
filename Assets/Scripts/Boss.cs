using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy
{
    public GameObject missile;
    public Transform missilePortA;
    public Transform missilePortB;
    public Transform[] enemyZones;
    public GameObject[] enemies;
    public List<int> enemyList;
    public bool isLook;
    public bool isStart;
    
    public GameObject tauntRangeIndicator; // 도발 범위 표시기에 대한 참조
    Vector3 lookVec;
     private bool hasStarted; // 코루틴 시작 여부를 추적하는 변수
    // Vector3 tauntVec;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        enemyList = new List<int>();

        nav.isStopped = true;
        hasStarted = false; // 초기에는 코루틴이 시작되지 않았음을 표시

    }

    // Update is called once per frame
    void Update()
    {
        if(isDead){
            StopAllCoroutines();
            return;
        }
        if(isLook){
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            lookVec = new Vector3(h,0,v) * 5f;
            transform.LookAt(target.position + lookVec);
        }
        if(isStart && !hasStarted){
            hasStarted = true;
            StartCoroutine(Think());
        }

        // else{
        //     nav.SetDestination(tauntVec);
        // }
    }

    IEnumerator Think(){
        yield return new WaitForSeconds(0.5f);

        int ranAction = Random.Range(0, 4);
        switch (ranAction){
            case 0:
            case 1:
                StartCoroutine(MissileShot());
                break;
            case 2:
                StartCoroutine(RockShot());
                break;
            case 3:
                StartCoroutine(Taunt());
                break;
            // case 4:
            //     StartCoroutine(GenerateEnemy());
            //     break;
        }
    }

    IEnumerator MissileShot(){
        anim.SetTrigger("doShot");
        yield return new WaitForSeconds(0.2f);
        GameObject instantMissileA = Instantiate(missile, missilePortA.position, missilePortA.rotation);
        BossMissile bossMissileA = instantMissileA.GetComponent<BossMissile>();
        bossMissileA.target = target;

        yield return new WaitForSeconds(0.3f);
        GameObject instantMissileB = Instantiate(missile, missilePortB.position, missilePortB.rotation);
        BossMissile bossMissileB = instantMissileB.GetComponent<BossMissile>();
        bossMissileB.target = target;
        yield return new WaitForSeconds(2.5f);
        

        StartCoroutine(Think());
        
    }
    IEnumerator RockShot(){
        isLook = false;
        anim.SetTrigger("doBigShot");
        Instantiate(bullet, transform.position, transform.rotation);
        yield return new WaitForSeconds(3f);
        isLook = true;
        StartCoroutine(Think());

        
    }
    IEnumerator Taunt(){
        // tauntVec = target.position + lookVec;

        isLook = false;
        nav.isStopped = false;
        if (boxCollider != null)
        {
            boxCollider.enabled = false;
        }
        anim.SetTrigger("doTaunt");
        tauntRangeIndicator.SetActive(true); // 표시기를 활성화합니다.

        yield return new WaitForSeconds(1.5f);
        meleeArea.enabled = true;

        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;
        tauntRangeIndicator.SetActive(false); // 표시기를 비활성화합니다.

        yield return new WaitForSeconds(1f);
        isLook = true;
        nav.isStopped = true;
        boxCollider.enabled = true;
        StartCoroutine(Think());
    }

    // IEnumerator GenerateEnemy(){
    //     isLook = false;
    //     anim.SetTrigger("doShot");
    //     for(int index = 0; index < 3; index ++){
    //         int ran = Random.Range(0,3);
    //         enemyList.Add(ran);
    //     }

    //     while(enemyList.Count > 0){
    //         int ranZone = Random.Range(0,3);
    //         GameObject instantEnemy = Instantiate(enemies[enemyList[0]],enemyZones[ranZone].position, enemyZones[ranZone].rotation);
    //         Enemy enemy = instantEnemy.GetComponent<Enemy>();
    //         enemy.target = player.transform;
    //         enemyList.RemoveAt(0);
    //     }

    //     yield return new WaitForSeconds(3f);
    //     isLook = true;
    //     StartCoroutine(Think());

        
    // }
}
