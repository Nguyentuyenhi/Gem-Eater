using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum TileType
{
    Empty,
    Ground,// ô trống
    Obstacle,   // vật cản
    GemRed,     // đá quý đỏ
    GemWhite,
    GemYellow
}

public class GameManager : SingletonMonoBehaviour<GameManager>
{

    private bool[] isRunning;                         // đánh dấu cột nào đang chạy
    private Queue<int>[] columnQueues;                // queue để lưu các request
    private int height;
    private int width;
    [Header("Map Settings")]

    private Dictionary<TileType, GameObject> prefabDict;

    void Start()
    {
        height = MapManager.Instance.height;
        width = MapManager.Instance.width;

        isRunning = new bool[width];
        columnQueues = new Queue<int>[width];
        for (int i = 0; i < width; i++)
        {
            columnQueues[i] = new Queue<int>();
        }
        prefabDict = new Dictionary<TileType, GameObject> {
            { TileType.Ground, MapManager.Instance.groundPrefab },
            { TileType.Obstacle,  MapManager.Instance.obstaclePrefab },
            { TileType.GemRed,MapManager.Instance. gemRedPrefab },
            { TileType.GemWhite, MapManager.Instance.gemWhitePrefab },
            { TileType.GemYellow, MapManager.Instance.gemYellowPrefab }
        };

    }



    TileType GetRandomTile()
    {
        float rand = Random.value;
        if (rand < 0.1f) return TileType.Obstacle;   // 10% vật cản
        if (rand < 0.4f) return TileType.Ground;      // 30% ground
        if (rand < 0.6f) return TileType.GemRed;     // 20% đỏ
        if (rand < 0.8f) return TileType.GemWhite;
        return TileType.GemYellow;
    }





    public void TryCollapseColumn(Vector2Int nodePos)
    {
        int col = nodePos.x;
        int row = nodePos.y;

        // Nếu trong cột đó có Snake từ row trở xuống thì bỏ qua
        for (int y = row; y < height; y++)
        {
            Node node = MapManager.Instance.nodesMap[col, y];
            if (node.IsSnake()) return;
        }

        int lowestEmpty = -1;
        for (int y = 0; y <= row; y++)
        {
            Node node = MapManager.Instance.nodesMap[col, y];
            if (!node.IsSnake() && node.GetItemObject() == null && node.tileType == TileType.Empty)
            {
                lowestEmpty = y;
                break; 
            }
        }

        if (lowestEmpty != -1)
        {
            columnQueues[col].Enqueue(lowestEmpty);

            if (!isRunning[col])
            {
                StartCoroutine(RunQueue(col));
            }
        }
    }


    private IEnumerator RunQueue(int col)
    {
        isRunning[col] = true;

        while (columnQueues[col].Count > 0)
        {
            int rowStart = columnQueues[col].Dequeue();
            yield return CollapseColumn(col, rowStart);
        }

        isRunning[col] = false;
    }

    private IEnumerator CollapseColumn(int col, int rowStart)
    {
        int targetRow = rowStart;

        for (int y = rowStart; y < height; y++)
        {
            Node oldNode = MapManager.Instance.nodesMap[col, y];

            if (oldNode.IsSnake()) break;

            GameObject obj = oldNode.GetItemObject();
            if (obj == null) continue;

            TileType tileType = oldNode.GetTileType();
            Node newNode = MapManager.Instance.nodesMap[col, targetRow];
            targetRow++;
            if (newNode.IsSnake() || newNode.GetItemObject() != null)
                break;
            switch (oldNode.tileType)
            {
                case TileType.Obstacle:
                    oldNode.SetOccupied(false); break;

            }
            oldNode.ClearItem();


            Vector3 start = obj.transform.position;
            Vector3 end = newNode.transform.position;

            float t = 0;
            while (t < 1f)
            {
                t += Time.deltaTime * 10f;
                obj.transform.position = Vector3.Lerp(start, end, t);
                yield return null;
            }

            obj.transform.position = end;
            newNode.SetTile(tileType, obj);
            switch (newNode.tileType)
            {
                case TileType.Obstacle:
                    newNode.SetOccupied(true); break;

            }
        }

        SpawnerNewItem(col, targetRow);
    }

    private void SpawnerNewItem(int col, int targetRow)
    {
        for (int y = targetRow; y < height; y++)
        {
            Node node = MapManager.Instance.nodesMap[col, y];
            if (node.IsSnake() || node.GetItemObject() != null || node.tileType != TileType.Empty) continue;
            TileType newTile = GetRandomTile();
            if (prefabDict.TryGetValue(newTile, out var prefab))
            {
                Vector3 spawnPos = node.transform.position + Vector3.up * 1.5f;
                var newObj = Instantiate(prefab, spawnPos, Quaternion.identity, node.transform);

                MoveItem(newObj, node, spawnPos, newTile);
            }
        }
    }
    private void MoveItem(GameObject newObj, Node newNode, Vector3 spawnPos, TileType newTile)
    {
        Vector3 end = newNode.transform.position;
        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime * 10f;
            newObj.transform.position = Vector3.Lerp(spawnPos, end, t);
        }
        newObj.transform.position = end;

        newNode.SetTile(newTile, newObj);
    }
}
