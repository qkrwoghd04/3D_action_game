using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject menuCam;
    public GameObject gameCam;
    public Player player;
    public Boss boss;
    public int stage;
    public float playTime;
    public bool isBattle;
    public GameObject obstacleSpawner;

    [Header("Menu Panel")]
    public GameObject menuPanel;
    public GameObject gamePanel;
    public Text maxScoreTxt;

    [Header("Game Panel")]
    public Text scoreTxt;
    public Text stageTxt;
    public Text playTimeTxt;
    public Text playerHealthTxt;
    public Text playerAmmoTxt;
    public Text playerCoinTxt;
    public Image weapon1Img;
    public Image weapon2Img;
    public Image weapon3Img;
    public Image weapon4Img;
    public RectTransform bossHealthGroup;
    public RectTransform bossHealthBar;
    public RectTransform uiGroup;


    void Awake()
    {
        maxScoreTxt.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));
    }

    public void GameStart()
    {
        menuCam.SetActive(false);
        gameCam.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);
        obstacleSpawner.gameObject.SetActive(true);
    }

    public void Enter()
    {
        uiGroup.anchoredPosition = Vector3.zero;

    }

    public void Exit()
    {
        uiGroup.anchoredPosition = Vector3.down * 1000;
    }

    void Update()
    {
        if (isBattle)
        {
            playTime += Time.deltaTime;
        }
    }

    void LateUpdate()
    {
        // UI
        scoreTxt.text = string.Format("{0:n0}", player.score);
        stageTxt.text = "STAGE" + stage;
        int hour = (int)(playTime / 3600);
        int min = (int)((playTime - hour * 3600) / 60);
        int second = (int)(playTime % 60);

        // player UI
        playTimeTxt.text = string.Format("{0:00}", hour) + ":" + string.Format("{0:00}", min) + ":" + string.Format("{0:00}", second);
        playerHealthTxt.text = player.health + "/" + player.maxHealth;
        playerCoinTxt.text = string.Format("{0:n0}", player.coin);
        // if(player.equipSkill == null){
        //     playerAmmoTxt.text = "- / " + player.ammo;

        // }else if (player.equipSkill.type == Weapon.Type.Melee){
        //     playerAmmoTxt.text = "- / " + player.ammo;
        // }else{
        //     playerAmmoTxt.text = player.equipSkill.curAmmo + " / " + player.ammo;
        // }
        weapon1Img.color = new Color(1, 1, 1, player.hasSkills[0] ? 1 : 0);
        weapon2Img.color = new Color(1, 1, 1, player.hasSkills[1] ? 1 : 0);
        weapon3Img.color = new Color(1, 1, 1, player.hasSkills[2] ? 1 : 0);
        weapon4Img.color = new Color(1, 1, 1, player.hasSkills[3] ? 1 : 0);

        // boss health UI
        float bossHealthNormalized = Mathf.Max((float)boss.curHealth / boss.maxHealth, 0);
        bossHealthBar.localScale = new Vector3(bossHealthNormalized, 1, 1);
    }
}
