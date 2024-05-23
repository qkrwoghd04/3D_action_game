using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillCooldownUI : MonoBehaviour
{
    public Image cooldownImage; // 쿨타임 오버레이 이미지

    public void StartCooldown(float duration)
    {
        StartCoroutine(CooldownCoroutine(duration));
    }

    private IEnumerator CooldownCoroutine(float duration)
    {
        cooldownImage.fillAmount = 0;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cooldownImage.fillAmount = elapsed / duration;
            yield return null;
        }
        cooldownImage.fillAmount = 1;
    }
}
