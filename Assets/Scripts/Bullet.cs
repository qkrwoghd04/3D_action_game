using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public bool isMelee;
    public bool isRock;
    public bool isMissile;
    public bool isTaunt;

    void OnCollisionEnter(Collision collision) {
        if(!isRock && collision.gameObject.tag == "Floor"){
            Destroy(gameObject, 3);
        } 
    }

    void OnTriggerEnter(Collider other){
        if((!isTaunt || isMelee) && other.gameObject.tag == "Wall" || (!isTaunt || isMissile) && other.gameObject.tag == "Wall"){
            Destroy(gameObject);
        }
    }

    
}
