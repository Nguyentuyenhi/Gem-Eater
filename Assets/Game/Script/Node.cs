using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Node : MonoBehaviour
{
    [SerializeField] public int indexX;
    [SerializeField] public int indexY;
    [SerializeField] private bool isObstacle;
    [SerializeField] private Node previousNode;
    [SerializeField] private List<Node> neigboorNodes = new List<Node>();
    private SpriteRenderer spriteRenderer;
   // public List<ItemSO> itemSOList = new List<ItemSO>();  // Danh sách các ItemSO
    [SerializeField] private bool isSnake;
    [SerializeField] private bool isFood;
    private GameObject itemObject;




    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }



    public void SetIndex(int indexX, int indexY)
    {
        this.indexX = indexX;
        this.indexY = indexY;
        
    }

    public bool IsSnake() => isSnake;  
    public void SetIsSnake(bool value) => isSnake = value; 

    public bool GetIsSnake() => isSnake;  

    public void InitNode(Node[,] nodesMap)
    {
        SetNeighboorNode(indexX + 1, indexY, nodesMap);
        SetNeighboorNode(indexX - 1, indexY, nodesMap);
        SetNeighboorNode(indexX, indexY + 1, nodesMap);
        SetNeighboorNode(indexX, indexY - 1, nodesMap);
    }

    public void SetColor(Color color)
    {
        if (spriteRenderer != null)
            spriteRenderer.color = color;
    }

    public void SetNeighboorNode(int x, int y, Node[,] nodesMap)
    {
        if (x < 0 || y < 0 || x >= nodesMap.GetLength(0) || y >= nodesMap.GetLength(1)) return;
        neigboorNodes.Add(nodesMap[x, y]);
    }

    public void SetItemObject(GameObject item)
    {
        itemObject = item;
        Debug.Log($"Node ({indexX}, {indexY}) item set to {item?.name ?? "null"}");
    }

    public GameObject GetItemObject() => itemObject;

    public int GetIndexX() => indexX;
    public int GetIndexY() => indexY;

    public List<Node> GetNeighborNode() => neigboorNodes;

    public bool GetIsObstacle() => isObstacle;

    public void SetPreviousNode(Node previousNode) => this.previousNode = previousNode;
    public SpriteRenderer GetSpriteRenderer() => spriteRenderer;

    public void SetOccupied(bool occupied)
    {
        isObstacle = occupied;
        Debug.Log($"Node ({indexX}, {indexY}) occupancy set to {occupied}");
    }
}
