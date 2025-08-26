using UnityEngine;

public enum TileType { Empty = 0, Wall = 1, Food = 2 }

public class GridManager : MonoBehaviour
{
    public int width = 11;
    public int height = 11;
    public float cellSize = 1f;

    // prefabs
    public GameObject wallPrefab;
    public GameObject foodPrefab;
    public Transform gridParent;

    private TileType[,] grid;

    private GameObject[,] visuals;

    void Awake()
    {
        InitGrid();
    }

    public void InitGrid()
    {
        grid = new TileType[width, height];
        visuals = new GameObject[width, height];

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                grid[x, y] = TileType.Empty;

        for (int x = 0; x < width; x++)
        {
            SetTileRaw(x, 0, TileType.Wall);
            SetTileRaw(x, height - 1, TileType.Wall);
        }
        for (int y = 0; y < height; y++)
        {
            SetTileRaw(0, y, TileType.Wall);
            SetTileRaw(width - 1, y, TileType.Wall);
        }

        SetTileRaw(3, 3, TileType.Wall);
        SetTileRaw(4, 3, TileType.Wall);
        SetTileRaw(5, 3, TileType.Wall);
        SetTileRaw(7, 6, TileType.Wall);

        SetTile(2, 2, TileType.Food);
        SetTile(8, 2, TileType.Food);
        SetTile(6, 8, TileType.Food);
    }

    public Vector3 GridToWorld(Vector2Int g)
    {
        // center map at origin
        float offsetX = -(width - 1) * 0.5f * cellSize;
        float offsetY = -(height - 1) * 0.5f * cellSize;
        return new Vector3(offsetX + g.x * cellSize, offsetY + g.y * cellSize, 0f);
    }

    public bool InBounds(Vector2Int p)
    {
        return p.x >= 0 && p.y >= 0 && p.x < width && p.y < height;
    }

    public TileType GetTile(Vector2Int p)
    {
        if (!InBounds(p)) return TileType.Wall;
        return grid[p.x, p.y];
    }

    public void SetTile(int x, int y, TileType t)
    {
        SetTileRaw(x, y, t);
        SpawnVisual(x, y);
    }

    private void SetTileRaw(int x, int y, TileType t)
    {
        grid[x, y] = t;
        if (visuals[x, y] != null)
        {
            Destroy(visuals[x, y]);
            visuals[x, y] = null;
        }
    }

    private void SpawnVisual(int x, int y)
    {
        Vector2Int g = new Vector2Int(x, y);
        Vector3 pos = GridToWorld(g);
        GameObject prefab = null;
        if (grid[x, y] == TileType.Wall) prefab = wallPrefab;
        else if (grid[x, y] == TileType.Food) prefab = foodPrefab;

        if (prefab != null)
        {
            var go = Instantiate(prefab, pos, Quaternion.identity, gridParent);
            visuals[x, y] = go;
        }
    }

    // helper when outside wants to set
    public void SetTile(Vector2Int p, TileType t)
    {
        if (!InBounds(p)) return;
        SetTile(p.x, p.y, t);
    }

    // consume food; returns true if eaten
    public bool TryConsumeFood(Vector2Int p)
    {
        if (!InBounds(p)) return false;
        if (grid[p.x, p.y] == TileType.Food)
        {
            SetTile(p.x, p.y, TileType.Empty);
            return true;
        }
        return false;
    }

    // optional: expose is walkable
    public bool IsWalkable(Vector2Int p)
    {
        if (!InBounds(p)) return false;
        return grid[p.x, p.y] != TileType.Wall;
    }
}
