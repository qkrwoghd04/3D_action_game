using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class MapGenerator : MonoBehaviour
{
    public int mapSize = 31;
    public GameObject obstaclePrefab;
    public GameObject tallObstaclePrefab;
    public NavMeshSurface navMeshSurface; 
    private void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        // Create base map plane
        GameObject mapPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        mapPlane.transform.localScale = new Vector3(mapSize / 10.0f, 1, mapSize / 10.0f);
        mapPlane.transform.position = new Vector3(mapSize / 2, 0, mapSize / 2);

        // Generate obstacles
        GenerateObstacles();

         // Bake the NavMesh
        navMeshSurface.BuildNavMesh();
    }

    void GenerateObstacles()
    {
        int obstacleCount = 20;
        int tallObstacleCount = 5;
        int obstaclesGenerated = 0;
        int tallObstaclesGenerated = 0;

        while (obstaclesGenerated < obstacleCount || tallObstaclesGenerated < tallObstacleCount)
        {
            int x = Random.Range(0, mapSize);
            int z = Random.Range(0, mapSize);

            // Avoid placing obstacles in the center (Boss area)
            if (x >= mapSize / 2 - 2 && x <= mapSize / 2 + 2 && z >= mapSize / 2 - 2 && z <= mapSize / 2 + 2)
            {
                continue;
            }

            // Choose obstacle type
            GameObject obstacle;
            if (tallObstaclesGenerated < tallObstacleCount && Random.value < 0.25f)
            {
                obstacle = Instantiate(tallObstaclePrefab);
                obstacle.transform.position = new Vector3(x, 1, z);
                tallObstaclesGenerated++;
            }
            else if (obstaclesGenerated < obstacleCount)
            {
                obstacle = Instantiate(obstaclePrefab);
                obstacle.transform.position = new Vector3(x, 0.5f, z);
                obstaclesGenerated++;
            }
        }
    }
}
