using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab;
    public int obstacleCount = 20;
    public int tallObstacleCount = 5;

    private float bossRadius = 2f; // 보스의 반경

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

                obstaclesPlaced++;
            }
        }
    }
}
