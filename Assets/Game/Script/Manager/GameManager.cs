using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum TileType
{
    Empty,
    Ground,
    Obstacle,
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
    private Dictionary<TileType, GameObject> prefabDict;
    [Header("Score")]
    [SerializeField] private TMP_Text scorePrefab;



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
            if (oldNode.IsSnake()) yield break;

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

            MoveItem(obj, newNode, tileType);

        }

        SpawnerNewItem(col, targetRow);

    }

    private void SpawnerNewItem(int col, int targetRow)
    {
        for (int y = targetRow; y < height; y++)
        {
            Node node = MapManager.Instance.nodesMap[col, y];

            if (node.IsSnake() || node.GetItemObject() != null || node.tileType != TileType.Empty)
                continue;

            TileType newTile = GetRandomTile();
            if (prefabDict.TryGetValue(newTile, out var prefab))
            {
                Vector3 start = node.transform.position + Vector3.up * 1.5f;
                var newObj = Instantiate(prefab, start, Quaternion.identity, node.transform);

                MoveItem(newObj, node, newTile);

            }
        }
    }

    private void MoveItem(GameObject obj, Node newNode, TileType newTile)
    {
        Vector3 end = newNode.transform.position;
        newNode.SetTile(newTile, obj);

        if (newNode.tileType == TileType.Obstacle)
            newNode.SetOccupied(true);
        obj.transform.DOMove(end, 0.2f)
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                if (newNode.IsSnake())
                {
                    obj.transform.DOKill();
                    Destroy(obj);
                }
            })
            .OnComplete(() =>
            {
                obj.transform.position = end;
                if (newNode.IsSnake())
                {
                    Destroy(obj);
                }

            });
    }


    public int GetGemScore(TileType gemType, int combo)
    {
        int baseScore = 0;

        switch (gemType)
        {
            case TileType.GemRed: baseScore = 1; break;
            case TileType.GemYellow: baseScore = 2; break;
            case TileType.GemWhite: baseScore = 4; break;
        }

        if (combo == 1) return baseScore;
        return baseScore * (combo - 1) * 10;
    }

    public void SpawnerScore(Vector3 pos, int score)
    {
        TMP_Text scoreText = Instantiate(scorePrefab, pos, Quaternion.identity);
        scoreText.text = score.ToString();

        scoreText.DOFade(0, 2f).OnComplete(() => Destroy(scoreText.gameObject));
    }

}
