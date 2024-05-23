using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalBullet : MonoBehaviour
{
    public ElementalType type;
    public float effectDuration = 3f; // 색상 변경 지속 시간

    void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            switch (type)
            {
                case ElementalType.Ice:
                    StartCoroutine(ChangeColorTemporarily(enemy, Color.blue));
                    break;
                case ElementalType.Fire:
                    StartCoroutine(ChangeColorTemporarily(enemy, Color.red));
                    break;
                case ElementalType.Poison:
                    StartCoroutine(ChangeColorTemporarily(enemy, Color.green));
                    break;
            }
        }

        Destroy(gameObject);
    }

    private IEnumerator ChangeColorTemporarily(Enemy enemy, Color color)
    {
        Renderer[] renderers = enemy.GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            foreach (Renderer renderer in renderers)
            {
                renderer.material.color = color;
            }
            yield return new WaitForSeconds(effectDuration);
            foreach (Renderer renderer in renderers)
            {
                renderer.material.color = Color.white; // 원래 색상으로 되돌리기
            }
        }
        else
        {
            Debug.LogWarning("No Renderer found on the enemy object or its children.");
        }
    }
}
