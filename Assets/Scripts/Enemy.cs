using System.Collections;
using System.Collections.Generic;
using JetBrains.Rider.Unity.Editor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type {A, B ,C}
    public Type enemyType;
    public int maxHealth;
    public int curHealth;
    public Transform target;
    public GameObject bullet;
    public bool isChase;
    public bool isAttack;
    public BoxCollider meleeArea;

    Rigidbody rigid;
    BoxCollider boxCollider;
    Material mat;
    NavMeshAgent nav;
    Animator anim;

    void Awake(){
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponentInChildren<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        Invoke("ChaseStart", 2);
        // nav.updatePosition = false;
    }

    void FixedUpdate(){
        Targeting();
        FreezeVelocity();
    }

    void Targeting(){
        
        float targetRadius = 0f;
        float targetRange = 0f;
        

        switch(enemyType){
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

        if(rayHits.Length > 0 && !isAttack){
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack(){
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);
        
        switch(enemyType){
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
                meleeArea.enabled = false;

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

    void ChaseStart(){
        if (target != null) {
            isChase = true;
            anim.SetBool("isWalk", true);
        } else {
            Debug.LogError("Target is not assigned.");
        }
    }
    void Update(){
        if(nav.enabled){
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        } 
    }

    void FreezeVelocity()
    {
        if(isChase){
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }

    void OnTriggerEnter(Collider other) {
        if(other.tag == "Melee"){
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            StartCoroutine(OnDamage());
            Debug.Log("Melee: " + curHealth);
        }
        else if(other.tag == "Bullet"){
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            Destroy(other.gameObject);
            StartCoroutine(OnDamage());
            Debug.Log("Range: " + curHealth);
        }
    }

    IEnumerator OnDamage(){
        mat.color = Color.yellow;
        yield return new WaitForSeconds(0.1f);

        if(curHealth > 0){
            mat.color = Color.white;
        }
        else{
            mat.color = Color.gray;
            isChase = false;
            nav.enabled = false;
            anim.SetTrigger("doDie");
            Destroy(gameObject, 4);
        }
    }

    IEnumerator OnDamageGrenade(Vector3 reactVec){
        mat.color = Color.yellow;
        yield return new WaitForSeconds(0.1f);

        if(curHealth > 0){
            mat.color = Color.white;
        }
        else{
            mat.color = Color.gray;
            Destroy(gameObject, 4);
        }
    }

    public void HitByGrenade(Vector3 explosionPos){
        curHealth -= 100;
        Vector3 reactVec = transform.position - explosionPos;
        StartCoroutine(OnDamageGrenade(reactVec));
    }
}
