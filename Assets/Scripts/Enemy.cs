using System.Collections;
using System.Collections.Generic;
using JetBrains.Rider.Unity.Editor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type { A, B, C, D }
    public Type enemyType;
    public int maxHealth;
    public int curHealth;
    public int score;
    public Transform target;
    public GameObject bullet;
    // public GameObject[] coin;

    public bool isChase;
    public bool isAttack;
    public bool isDead;
    
    public BoxCollider meleeArea;
    public BoxCollider boxCollider;
    public Rigidbody rigid;
    public MeshRenderer[] meshs;
    public NavMeshAgent nav;
    public Animator anim;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        if (enemyType != Type.D){
            Invoke("ChaseStart", 1);    
        }
        // nav.updatePosition = false;
    }

    void Update()
    {
        if (nav.enabled && enemyType != Type.D)
        {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        }

    }
    void FixedUpdate()
    {
        Targeting();
        FreezeVelocity();
    }
    /// <summary>
    /// 몬스터들이 사용자를 추적하는 함수 각 enemy에 따라 로직이 다름 IEnumerator Attack을 호출함
    /// </summary>
    void Targeting()
    {

        if (!isDead && enemyType != Type.D)
        {
            float targetRadius = 0f;
            float targetRange = 0f;


            switch (enemyType)
            {
                case Type.A:
                    targetRadius = 1.5f;
                    targetRange = 3f;
                    break;
                case Type.B:
                    targetRadius = 1f;
                    targetRange = 12f;
                    break;
                case Type.C:
                    targetRadius = 0.5f;
                    targetRange = 25f;
                    break;
            }
            RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));

            if (rayHits.Length > 0 && !isAttack)
            {
                StartCoroutine(Attack());
            }
        }
    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);

        switch (enemyType)
        {
            case Type.A:
                yield return new WaitForSeconds(0.2f);

                meleeArea.enabled = true;
                yield return new WaitForSeconds(1f);

                meleeArea.enabled = false;

                yield return new WaitForSeconds(1f);
                break;
            case Type.B:
                yield return new WaitForSeconds(0.1f);
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(0.5f);
                rigid.velocity = Vector3.zero;
                if (meleeArea != null)
                {
                    meleeArea.enabled = false;
                }


                yield return new WaitForSeconds(2f);
                break;
            case Type.C:
                yield return new WaitForSeconds(0.5f);
                GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 20;
                yield return new WaitForSeconds(2f);
                break;
        }

        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);
    }

    /// <summary>
    /// animation 을 호출하고 따라감
    /// </summary>
    void ChaseStart()
    {
        if (target != null)
        {
            isChase = true;
            anim.SetBool("isWalk", true);
        }
        else
        {
            Debug.LogError("Target is not assigned.");
        }
    }

    void FreezeVelocity()
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            StartCoroutine(OnDamage());
            Debug.Log("Melee: " + curHealth);
        }
        else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            Destroy(other.gameObject);
            StartCoroutine(OnDamage());
            Debug.Log("Range: " + curHealth);
        }
    }

    IEnumerator OnDamage()
    {
        foreach (MeshRenderer mesh in meshs) mesh.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if (curHealth > 0)
        {
            foreach (MeshRenderer mesh in meshs) mesh.material.color = Color.white;
        }
        else
        {
            foreach (MeshRenderer mesh in meshs) mesh.material.color = Color.gray;
            isDead = true;
            isChase = false;
            nav.enabled = false;
            anim.SetTrigger("doDie");
             //동전 떨구는 로직
            // Player player = target.GetComponent<Player>();
            // player.score += score;
            // int ranCoin = Random.Range(0,3);
            // Instantiate(coin[ranCoin], transform.position, Quaternion.identity);
            if (enemyType != Type.D) Destroy(gameObject, 4);
        }
    }

    public void HitByGrenade(Vector3 explosionPos)
    {
        curHealth -= 100;
        Vector3 reactVec = transform.position - explosionPos;
        StartCoroutine(OnDamageGrenade(reactVec));
    }
    IEnumerator OnDamageGrenade(Vector3 reactVec)
    {
        foreach (MeshRenderer mesh in meshs) mesh.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        // Apply reaction force
        if (rigid != null)
        {
            rigid.AddForce(reactVec.normalized * 10, ForceMode.Impulse); // 밀려나는 힘 적용
        }

        if (curHealth > 0)
        {
            foreach (MeshRenderer mesh in meshs) mesh.material.color = Color.white;
        }
        else
        {
            foreach (MeshRenderer mesh in meshs) mesh.material.color = Color.gray;
            isDead = true;
            isChase = false;
            nav.enabled = false;
            anim.SetTrigger("doDie");
            //동전 떨구는 로직
            // Player player = target.GetComponent<Player>();
            // player.score += score;
            // int ranCoin = Random.Range(0,3);
            // Instantiate(coin[ranCoin], transform.position, Quaternion.identity);
            if (enemyType != Type.D){
                Destroy(gameObject, 4);
            } 
        }
    }
}
