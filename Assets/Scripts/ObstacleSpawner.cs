using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab;
    public int mapSize = 31;
    public GameObject[] skillCoinPrefabs; // 4가지 Skill Coin 프리팹 배열
    private int maxSkillCoins = 3;
    private int currentSkillCoins = 0;
    private float spawnInterval = 10f; // Skill Coin 생성 간격
    public int obstacleCount = 20;
    public int tallObstacleCount = 5;

    private float bossRadius = 10f; // 보스의 반경
    private List<Vector3> obstaclePositions = new List<Vector3>(); // 장애물 위치 저장


    void Start()
    {
        int obstaclesPlaced = 0;

        while (obstaclesPlaced < obstacleCount)
        {
            float yPos = obstaclesPlaced < tallObstacleCount ? 5.5f : 3.5f;
            Vector3 position = new Vector3(Random.Range(-140, 140), yPos, Random.Range(-140, 140));

            // 보스 범위 내에 있는지 확인
            if (Mathf.Abs(position.x) > bossRadius || Mathf.Abs(position.z) > bossRadius)
            {
                GameObject obstacle = Instantiate(obstaclePrefab, position, Quaternion.identity);
                obstacle.transform.localScale = new Vector3(1, obstaclesPlaced < tallObstacleCount ? 2 : 1, 1);

                obstaclePositions.Add(position); // 장애물 위치 저장
                obstaclesPlaced++;
            }
        }
        StartCoroutine(SpawnSkillCoin());
    }

    IEnumerator SpawnSkillCoin()
    {
        while (currentSkillCoins < maxSkillCoins)
        {
            yield return new WaitForSeconds(spawnInterval);

            Vector3 spawnPosition;
            bool validPosition;

            // 위치가 유효한지 확인
            do
            {
                spawnPosition = new Vector3(Random.Range(-140, 140), 3, Random.Range(-140, 140));

                validPosition = true;

                // 보스 위치와 가까운지 확인
                if (Vector3.Distance(spawnPosition, new Vector3(4, 3, 4)) <= bossRadius)
                {
                    validPosition = false;
                }

                // 장애물 위치와 가까운지 확인
                foreach (Vector3 obstaclePos in obstaclePositions)
                {
                    if (Vector3.Distance(spawnPosition, obstaclePos) <= bossRadius)
                    {
                        validPosition = false;
                        break;
                    }
                }

            } while (!validPosition);

            // Randomly select a Skill Coin prefab
            int randomIndex = Random.Range(0, skillCoinPrefabs.Length);
            GameObject selectedSkillCoinPrefab = skillCoinPrefabs[randomIndex];

            // Instantiate Skill Coin
            Instantiate(selectedSkillCoinPrefab, spawnPosition, Quaternion.identity);
            currentSkillCoins++;

            // Ensure only one Skill Coin at a time
            yield return new WaitUntil(() => currentSkillCoins < maxSkillCoins);
        }
    }
}