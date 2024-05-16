using System.Collections;
using System.Collections.Generic;
using JetBrains.Rider.Unity.Editor;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;

    Rigidbody rigid;
    BoxCollider boxCollider;
    Material mat;

    void Awake(){
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponent<MeshRenderer>().material;
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
            Destroy(gameObject, 4);
        }
    }
}
