using System.Collections;
using System.Collections.Generic;
using JetBrains.Rider.Unity.Editor;
using UnityEngine;

public class Gerenade : MonoBehaviour
{
    public GameObject meshObj;
    public GameObject effectObj;
    public Rigidbody rigid;
    void Start()
    {
        StartCoroutine(Explosion());
    }

    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(3f);
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;

        meshObj.SetActive(false);
        effectObj.SetActive(true);

        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 30, Vector3.up, 0f, LayerMask.GetMask("Enemy"));

        foreach(RaycastHit hitObj in rayHits){
            hitObj.transform.GetComponent<Enemy>().HitByGrenade(transform.position);
        }
        Destroy(gameObject, 4);
    }
}
