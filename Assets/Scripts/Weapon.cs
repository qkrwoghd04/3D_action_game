using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Shot, ParabolaShot, Throw }
    public Type type;
    public int damage;
    public float attackSpeed;
    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;
    public Transform bulletPos;
    public GameObject bullet;
    public Transform bulletCasePos;
    public GameObject bulletCase;
    public GameObject lightningEffectPrefab; // Add reference to the lightning effect prefab
    public float lightningStrikeRadius = 100f; // Radius to detect enemies for lightning strike



    public void Use()
    {
        if (type == Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
        else if (type == Type.Shot)
        {
            StopCoroutine("Shot");
            StartCoroutine("Shot");
        }
        else if (type == Type.ParabolaShot)
        {
            StopCoroutine("ParabolaShot");
            StartCoroutine("ParabolaShot");
        }
        else if (type == Type.Throw)
        {
            StopCoroutine("Throw");
            StartCoroutine("Throw");
        }
    }

    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = true;
        trailEffect.enabled = true;

        // Call the Lightning Strike effect
        StartCoroutine(LightningStrike());

        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;
    }

    IEnumerator LightningStrike()
    {
        // Detect all enemies within the specified radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, lightningStrikeRadius, LayerMask.GetMask("Enemy"));

        foreach (var hitCollider in hitColliders)
        {
            // Instantiate the lightning effect above the enemy
            Vector3 lightningPosition = hitCollider.transform.position + Vector3.up * 15; // Move 20 units up on the y-axis
            Quaternion rotation = Quaternion.Euler(90, 0, 0); // Rotate 90 degrees around the x-axis
            GameObject lightningEffect = Instantiate(lightningEffectPrefab, lightningPosition, rotation);

            // Destroy the lightning effect after 1 second
            Destroy(lightningEffect, 3f);

            // Apply damage to the enemy (assuming the enemy has a method to take damage)
            Enemy enemy = hitCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage); // Ensure Enemy script has a TakeDamage method
            }
        }

        yield return null;
    }



    IEnumerator Shot()
    {
        GameObject instantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 70;

        yield return new WaitForSeconds(0.1f);
        GameObject instantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = instantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3);
        caseRigid.AddForce(caseVec, ForceMode.Impulse);
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);
    }

    IEnumerator ParabolaShot()
    {
        GameObject instantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 25 + bulletPos.up * 15;

        yield return new WaitForSeconds(0.1f);
        GameObject instantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = instantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3);
        caseRigid.AddForce(caseVec, ForceMode.Impulse);
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);
    }

    IEnumerator Throw()
    {
        GameObject instantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        Vector3 throwDirection = bulletPos.forward * 25 + bulletPos.up * 5; // 수직 성분을 20으로 설정
        bulletRigid.velocity = throwDirection;

        yield return new WaitForSeconds(0.1f);
        GameObject instantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = instantCase.GetComponent<Rigidbody>();

        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3);
        caseRigid.AddForce(caseVec, ForceMode.Impulse);
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);
    }


}
