using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static UnityEditor.VersionControl.Asset;

public class MapManager : SingletonMonoBehaviour<MapManager>
{
    public int width = 20;
    public int height = 20;
    public GameObject nodePrefab;
    public GameObject snakePrefab;
    public GameObject foodPrefab;
    public GameObject ObstaclePrefab;

    public Node[,] nodesMap;


    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        nodesMap = new Node[width, height];


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = new Vector3(x * 1.1f, y * 1.1f, 0);

                var nodeObj = Instantiate(nodePrefab, pos, Quaternion.identity, transform);

                Node node = nodeObj.GetComponent<Node>();
                node.SetIndex(x, y);

                nodesMap[x, y] = node;
            }
        }


        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                nodesMap[x, y].InitNode(nodesMap);

        GenerateCustomMap();
    }

    public void SpawnSnake(Vector2Int pos)
    {
        Node node = nodesMap[pos.x, pos.y];
        var snake = Instantiate(snakePrefab, node.transform.position, Quaternion.identity);
        node.SetIsSnake(true);
        SnakeController controller = snake.GetComponent<SnakeController>();
        controller.SetCurrentNode(node);
    }

    public void SpawnFood(Vector2Int pos)
    {
        Node node = nodesMap[pos.x, pos.y];
        var food = Instantiate(foodPrefab, node.transform.position, Quaternion.identity, node.transform);
        node.SetItemObject(food);
    }

    public void SpawnObstacle(Vector2Int pos)
    {
        Node node = nodesMap[pos.x, pos.y];
        var Obstacle = Instantiate(ObstaclePrefab, node.transform.position, Quaternion.identity, node.transform);
        node.SetOccupied(true);
    }
    void GenerateCustomMap()
    {
       SpawnSnake(new Vector2Int(7, 7));

        SpawnSnake(new Vector2Int(7, 5));

        // Spawn obstacles (nâu)
        SpawnObstacle(new Vector2Int(3, 3));
        SpawnObstacle(new Vector2Int(5, 3));
        SpawnObstacle(new Vector2Int(3, 4));
        SpawnObstacle(new Vector2Int(3, 5));
        SpawnObstacle(new Vector2Int(3, 6));
      SpawnObstacle(new Vector2Int(4, 6));

        // Spawn food (hong)
       SpawnFood(new Vector2Int(7, 3) );
       SpawnFood(new Vector2Int(2, 2));
    }
    public Node GetNearestNode(Vector3 worldPos)
    {
        float minDist = float.MaxValue;
        Node nearest = null;

        foreach (var node in nodesMap)
        {
            float dist = Vector3.Distance(worldPos, node.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = node;
            }
        }
        return nearest;
    }




}

