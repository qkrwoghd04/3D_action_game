using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject menuCam;
    public GameObject gameCam;
    public Player player;
    public Boss boss;
    public int stage;
    public float playTime;
    public bool isBattle;
    public bool isTraining;
    public GameObject obstacleSpawner;

    [Header("Menu Panel")]
    public GameObject menuPanel;
    public GameObject gamePanel;
    public GameObject overPanel;
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
    public Text curScroeText;
    public Text bestText;


    void Awake()
    {
        maxScoreTxt.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));

        if(PlayerPrefs.HasKey("MaxScore")){
            PlayerPrefs.SetInt("MaxScore", 0);
        }
    }

    public void GameStart()
    {
        menuCam.SetActive(false);
        gameCam.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        isBattle = true;
        boss.isStart = true;
        player.gameObject.SetActive(true);
        obstacleSpawner.gameObject.SetActive(true);

        isTraining = false;
        AudioManager.instance.PlayBgm(true);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }
    
    public void GameOver()
    {   
        isBattle = false;
        gamePanel.SetActive(false);
        overPanel.SetActive(true);
        curScroeText.text = scoreTxt.text;

        int maxScore = PlayerPrefs.GetInt("MaxScore");
        if(player.score > maxScore){
            bestText.gameObject.SetActive(true);
            PlayerPrefs.SetInt("MaxScore", player.score);
        }
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Lose);

    }

    public void TrainingGround(){
        menuCam.SetActive(false);
        gameCam.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);

        player.hasSkills[0] = true;
        player.hasSkills[1] = true;
        player.hasSkills[2] = true;
        player.hasSkills[3] = true;

        weapon1Img.color = new Color(1, 1, 1, player.hasSkills[0] ? 1 : 0);
        weapon2Img.color = new Color(1, 1, 1, player.hasSkills[1] ? 1 : 0);
        weapon3Img.color = new Color(1, 1, 1, player.hasSkills[2] ? 1 : 0);
        weapon4Img.color = new Color(1, 1, 1, player.hasSkills[3] ? 1 : 0);

        isTraining = true;
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    public void ReStart(){
        SceneManager.LoadScene(0);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
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

        // 트레이닝 모드에서 체력 관리
        if (isTraining)
        {
            player.health = player.maxHealth;
            boss.curHealth = boss.maxHealth; 
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
