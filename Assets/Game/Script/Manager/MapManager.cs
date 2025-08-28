using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static UnityEditor.PlayerSettings;
using static UnityEditor.VersionControl.Asset;
[System.Serializable]
public class SnakeInfo
{
    public Vector2Int headPos;
    public int[,] snakeData;
    public SnakeController.Direction direction;
}

public class MapManager : SingletonMonoBehaviour<MapManager>
{
    public int width = 15;
    public int height = 15;
    public GameObject nodePrefab;
    public GameObject snakeHeadPrefab;
    public GameObject snakeTailPrefab;
    public GameObject snakeBodyPrefab;
    public GameObject foodPrefab;
    public GameObject ObstaclePrefab;
    public Node[,] nodesMap;

    public List<SnakeController> snakes = new List<SnakeController>();
    public List<Node> foodNodes = new List<Node>();

    void Start()
    {
        int[,] mapData = new int[,]
{
    {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,1,1,1,1,0,0,0,1,0,1,1,1,1,1},
    {1,1,1,1,1,0,1,0,0,0,1,1,1,1,1},
    {1,1,1,1,1,0,0,0,1,1,1,1,1,1,1},
    {1,1,1,1,1,1,1,0,2,1,1,1,1,1,1},
    {1,1,1,1,1,2,0,0,1,1,1,1,1,1,1},
    {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
};
        GenerateMapFromData(mapData);
    }

    public void GenerateMapFromData(int[,] data)
    {
        width = data.GetLength(0);
        height = data.GetLength(1);
        nodesMap = new Node[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = height - 1; y >= 0; y--)
            {
                int newX = y;
                int newY = width - 1 - x;

                Vector3 pos = new Vector3(newX * 1f, newY * 1f, 0);
                var nodeObj = Instantiate(nodePrefab, pos, Quaternion.identity, transform);
                Node node = nodeObj.GetComponent<Node>();
                node.SetIndex(x, y);

                if ((x + y) % 2 == 0)
                    node.SetColor(Color.white);
                else
                    node.SetColor(new Color(0.7f, 0.7f, 0.7f));

                nodesMap[x, y] = node;

                if (data[x, y] == 1)
                    SpawnObstacle(new Vector2Int(x, y));
                else if (data[x, y] == 2)
                    SpawnFood(new Vector2Int(x, y));
            }
        }
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                nodesMap[x, y].InitNode(nodesMap);
        Vector2Int headPos = new Vector2Int(6, 7);

        int[,] snakeData = new int[,]
        {
    { 5, 4, 4},
    { 0, 0, 3},
        };

        SpawnSnakeFromMatrix(headPos, snakeData, SnakeController.Direction.Down
            );
        SpawnSnakeFromMatrix(new Vector2Int(6, 8), new int[,]
        {
    { 0, 5 },
    { 3, 4},
        }, SnakeController.Direction.Left);


    }

    public void SpawnAllSnakes(List<SnakeInfo> snakes)
    {
        foreach (var s in snakes)
        {
            SpawnSnakeFromMatrix(s.headPos, s.snakeData, s.direction);
        }
    }
    public void SpawnSnakeFromMatrix(Vector2Int startPos, int[,] snakeData, SnakeController.Direction dir)
    {
        int width = snakeData.GetLength(0);
        int height = snakeData.GetLength(1);

        Vector2Int? headLocal = null;
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (snakeData[x, y] == 3)
                    headLocal = new Vector2Int(x, y);

        if (!headLocal.HasValue)
        {
            Debug.LogError("Không tìm thấy head trong snakeData!");
            return;
        }

        Vector2Int headWorld = startPos;

        bool[,] visited = new bool[width, height];

        Queue<(Vector2Int local, Vector2Int world)> q = new Queue<(Vector2Int, Vector2Int)>();
        q.Enqueue((headLocal.Value, headWorld));

        SnakeController headController = null;
        List<SnakeBody> parts = new List<SnakeBody>();

        while (q.Count > 0)
        {
            var (localPos, worldPos) = q.Dequeue();
            if (visited[localPos.x, localPos.y]) continue;
            visited[localPos.x, localPos.y] = true;

            int val = snakeData[localPos.x, localPos.y];
            Node node = nodesMap[worldPos.x, worldPos.y];
            node.SetIsSnake(true);

            if (val == 3)
            {
                var headObj = Instantiate(snakeHeadPrefab, node.transform.position, Quaternion.identity);
                headController = headObj.GetComponent<SnakeController>();
                headController.SetCurrentNode(node, dir);
                snakes.Add(headController);
            }
            else if (val == 4 || val == 5)
            {
                GameObject prefab = val == 4 ? snakeBodyPrefab : snakeTailPrefab;
                var partObj = Instantiate(prefab, node.transform.position, Quaternion.identity);
                SnakeBody part = partObj.GetComponent<SnakeBody>();
                part.SetCurrentNode(node);
                parts.Add(part);
            }

            Vector2Int[] dirs = new Vector2Int[]
            {
            new Vector2Int(1,0), new Vector2Int(-1,0),
            new Vector2Int(0,1), new Vector2Int(0,-1)
            };

            foreach (var d in dirs)
            {
                Vector2Int nextLocal = localPos + d;
                Vector2Int nextWorld = worldPos + d;

                if (nextLocal.x >= 0 && nextLocal.x < width &&
                    nextLocal.y >= 0 && nextLocal.y < height)
                {
                    int v = snakeData[nextLocal.x, nextLocal.y];
                    if ((v == 4 || v == 5) && !visited[nextLocal.x, nextLocal.y])
                    {
                        q.Enqueue((nextLocal, nextWorld));
                    }
                }
            }
        }

        if (headController != null)
            headController.SetSnakaParts(parts);
    }

    public void SpawnFood(Vector2Int pos)
    {
        Node node = nodesMap[pos.x, pos.y];
        var food = Instantiate(foodPrefab, node.transform.position, Quaternion.identity, node.transform);
        node.SetItemObject(food);
        foodNodes.Add(node);
    }

    public void SpawnObstacle(Vector2Int pos)
    {
        Node node = nodesMap[pos.x, pos.y];
        var Obstacle = Instantiate(ObstaclePrefab, node.transform.position, Quaternion.identity, node.transform);
        node.SetOccupied(true);
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

