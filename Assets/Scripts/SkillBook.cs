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
    }
}
