using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBook : MonoBehaviour
{
    public RectTransform uiGroup;

    Player enterPlayer;

    void Enter(Player player){
        enterPlayer = player;
        uiGroup.anchoredPosition = Vector3.zero;

    }

    void Exit(){
        uiGroup.anchoredPosition = Vector3.down * 1000;
    }
}
