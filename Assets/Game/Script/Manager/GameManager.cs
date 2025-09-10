using System.Collections;
using System.Collections.Generic;
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
    [Header("Map Settings")]

    private Dictionary<TileType, GameObject> prefabDict;

    void Start()
    {
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
        if (rand < 0.4f) return TileType.Empty;      // 30% ô trống
        if (rand < 0.6f) return TileType.GemRed;     // 20% đỏ
        if (rand < 0.8f) return TileType.GemWhite;    // 20% xanh
        return TileType.GemYellow;                   // 20% lục
    }

    public void TryCollapseColumn(Vector2Int nodePos)
    {
        int col = nodePos.x;
        int row = nodePos.y;
        for (int y = 0; y < MapManager.Instance.height; y++)
        {
            Node node = MapManager.Instance.nodesMap[col, y];
            if (node.IsSnake()) return;
        }
        StartCoroutine(CollapseColumn(col, row));   

    }

    private IEnumerator CollapseColumn(int col, int rowStart)
    {
        int height = MapManager.Instance.height;
        List<(TileType, GameObject)> items = new List<(TileType, GameObject)>();

        // B1: Lấy tất cả item còn lại trong cột từ rowStart trở lên
        for (int y = rowStart; y < height; y++)
        {
            Node node = MapManager.Instance.nodesMap[col, y];
            if (node.GetItemObject() != null)
            {
                items.Add((node.GetTileType(), node.GetItemObject()));
                node.ClearItem();
            }
        }

        // B2: Đưa chúng xuống từ dưới lên
        int targetRow = rowStart;
        foreach (var (tileType, obj) in items)
        {
            Node node = MapManager.Instance.nodesMap[col, targetRow];
            Vector3 start = obj.transform.position;
            Vector3 end = node.transform.position;

            float t = 0;
            while (t < 1f)
            {
                t += Time.deltaTime * 5f;
                obj.transform.position = Vector3.Lerp(start, end, t);
                yield return null;
            }
            obj.transform.position = end;
            node.SetTile(tileType, obj);
            targetRow++;
        }

        // B3: Nếu còn chỗ trống ở trên thì spawn item mới
        for (int y = targetRow; y < height; y++)
        {
            Node node = MapManager.Instance.nodesMap[col, y];
            TileType newTile = GetRandomTile();
            if (prefabDict.TryGetValue(newTile, out var prefab))
            {
                // Spawn từ "ngoài map" để rơi xuống cho đẹp
                Vector3 spawnPos = node.transform.position + Vector3.up * 1.5f;
                var newObj = Instantiate(prefab, spawnPos, Quaternion.identity, node.transform);

                // Animate rơi xuống
                Vector3 end = node.transform.position;
                float t = 0;
                while (t < 1f)
                {
                    t += Time.deltaTime * 5f;
                    newObj.transform.position = Vector3.Lerp(spawnPos, end, t);
                    yield return null;
                }
                newObj.transform.position = end;

                node.SetTile(newTile, newObj);
            }
        }
    }


}
