using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
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
    bool isSwap;
    bool isFireReady = true;
    float fireDelay;
    int equipIndex = -1;
    bool isBorder;
    bool isDamage;
    bool isDead;

    [Header("Player info")]
    public float speed;
    public int health;
    public int maxHealth;
    public int score;
    public int coin;
    public int ammo;
    public Weapon equipSkill;

    [Header("Others")]
    NavMeshAgent agent;
    Animator anim;
    MeshRenderer[] meshs;
    public GameManager manager;
    public Transform spot;
    LineRenderer lr;
    Coroutine draw;
    GameObject nearObject;
    Rigidbody rigid;


    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero;
    }
    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 3, Color.green);
        isBorder = Physics.Raycast(transform.position, transform.forward, 3, LayerMask.GetMask("Wall"));
    }
    void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();

        // 벽에 부딪혔을 때 이동 중지
        if (isBorder)
        {
            agent.isStopped = true;
            anim.SetBool("isRun", true); // 계속 "run" 상태 유지
        }
        else
        {
            agent.isStopped = false;
        }
    }
    void GetInput()
    {
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
        sDown4 = Input.GetButtonDown("Swap4");
    }

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        lr = GetComponent<LineRenderer>();
        rigid = GetComponent<Rigidbody>();
        meshs = GetComponentsInChildren<MeshRenderer>();

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
        Interact();
        Attack();
        
    }

    
    void ClickMove()
    {
        if (Input.GetMouseButtonDown(1) && !isDead)
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
    void OnTriggerEnter(Collider other) {
        if(other.tag == "EnemyBullet"){
            if(!isDamage){
                Bullet enemyBullet = other.GetComponent<Bullet>();
                health -= enemyBullet.damage;
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);
                StartCoroutine(OnDamage());
            }
            if(other.GetComponent<Rigidbody>() != null){
                Destroy(other.gameObject);
            }
        }
    }

    IEnumerator OnDamage(){
        isDamage = true;
        foreach(MeshRenderer mesh in meshs){
            mesh.material.color = Color.red;
        }
        if(health <= 0 && !isDead){
            OnDie();
        }
        yield return new WaitForSeconds(1f);

        isDamage = false;
        foreach(MeshRenderer mesh in meshs){
            mesh.material.color = Color.white;
        }

    }

    void OnDie(){
        anim.SetTrigger("doDie");
        isDead = true;
        manager.GameOver();
    }
    void SwapOut()
    {
        isSwap = false;
    }

    void Swap()
    {
        if(isDead) return;
        if (sDown1 && (!hasSkills[0] || equipIndex == 0)) return;
        if (sDown2 && (!hasSkills[1] || equipIndex == 1)) return;
        if (sDown3 && (!hasSkills[2] || equipIndex == 2)) return;
        if (sDown4 && (!hasSkills[3] || equipIndex == 3)) return;

        int skillIndex = -1;
        if (sDown1) skillIndex = 0;
        if (sDown2) skillIndex = 1;
        if (sDown3) skillIndex = 2;
        if (sDown4) skillIndex = 3;


        if (sDown1 || sDown2 || sDown3 || sDown4)
        {
            if (equipSkill != null) equipSkill.gameObject.SetActive(false);

            equipIndex = skillIndex;
            equipSkill = skills[skillIndex].GetComponent<Weapon>();
            skills[skillIndex].gameObject.SetActive(true);

            anim.SetTrigger("doSwap");

            isSwap = true;

            Invoke("SwapOut", 0.4f);
        }
    }
    void Interact()
    {
        if (!isDead && nearObject != null && nearObject.CompareTag("Skill"))
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

    void Attack()
    {
        if (equipSkill == null) return;

        fireDelay += Time.deltaTime;
        isFireReady = equipSkill.attackSpeed < fireDelay;

        if (isFireReady && !isSwap && Input.GetMouseButtonDown(0))
        {
            equipSkill.Use();
            anim.SetTrigger(equipSkill.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            fireDelay = 0;
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
        }
    }
}
