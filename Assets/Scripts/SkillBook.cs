using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillBook : MonoBehaviour
{
    public RectTransform uiGroup;

    public Player enterPlayer;

    public GameObject[] skillObj;
    public int[] skillCoin;
    public Text[] unlockTxt;
    public Text[] lockTxt;

    void Enter(Player player){
        enterPlayer = player;
        uiGroup.anchoredPosition = Vector3.zero;

    }

    void Exit(){
        uiGroup.anchoredPosition = Vector3.down * 1000;
    }

    public void Buy(int index){
        int coin = skillCoin[index];
        Debug.Log(coin);
        if(coin > enterPlayer.coin) {
            return;
        }

        enterPlayer.coin -= coin;
        enterPlayer.hasSkills[index] = true;

        // 업데이트된 스킬 상태를 UI에 반영
        UpdateSkillUI(index);
        
    }

    // 스킬의 잠금 해제 상태에 따라 UI 업데이트
    void UpdateSkillUI(int index) {
        if (enterPlayer.hasSkills[index]) {
            unlockTxt[index].gameObject.SetActive(true);
            lockTxt[index].gameObject.SetActive(false);
        } else {
            unlockTxt[index].gameObject.SetActive(false);
            lockTxt[index].gameObject.SetActive(true);
        }
    }
}
