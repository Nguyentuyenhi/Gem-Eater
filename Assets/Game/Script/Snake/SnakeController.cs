using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
public class SnakeController : SnakeBase
{
    private bool isDragging = false;
    private bool isMoving = false;
    [SerializeField] private Node oldNode;
    [SerializeField] private TileType curGem;

    [SerializeField] private int comboCount = 0;



    private void Start()
    {
        RotatingSnake(currentDirection);
    }
    private void Update()
    {
        if (isDragging && !isMoving)
        {
            HandleMouseDrag();
        }
    }

    public void SetCurrentNode(Node node, Direction dir)
    {
        currentNode = node;
        transform.position = node.transform.position;
        currentDirection = dir;
    }

    public void SetSnakaParts(List<SnakeBody> Parts)
    {
        GameManager.Instance.snakeParts = Parts;
    }
    private void OnMouseDown()
    {
        isDragging = true;
    }

    private void OnMouseUp()
    {
        isDragging = false;
    }

    void HandleMouseDrag()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.forward, Vector3.zero);

        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Node targetNode = MapManager.Instance.GetNearestNode(hitPoint);

            if (targetNode != null && targetNode != currentNode)
            {
                if (currentNode.GetNeighborNode().Contains(targetNode) &&
                    !targetNode.GetIsObstacle() && !targetNode.IsSnake())
                {
                    CheckDirection(targetNode);
                }
            }
        }
    }

    void MoveToNode(Node newNode)
    {
        currentNode.SetIsSnake(false);
        MoveSmooth(newNode);
    }


    public void MoveSmooth(Node newNode)
    {
        isMoving = true;

        oldNode = currentNode;

        newNode.SetIsSnake(true);
        distance = Vector3.Distance(transform.position, newNode.transform.position);
        duration = distance / moveSpeed;

        transform.DOMove(newNode.transform.position, duration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                currentNode = newNode;
                // currentNode.SetIsSnake(true);
                CheckNodeISGem(newNode);

                transform.position = newNode.transform.position;
                isMoving = false;
            });
        UpdateSnakeParts(oldNode);
        GameManager.Instance.CheckGameOver(newNode);
    }


    private void CheckNodeISGem(Node node)
    {
        var itemObj = node.GetItemObject();
        if (itemObj != null)
        {
            switch (node.tileType)
            {
                case TileType.GemRed:
                case TileType.GemYellow:
                case TileType.GemWhite:
                    if (node.tileType == curGem)
                    {
                        comboCount++;
                    }
                    else
                    {
                        curGem = node.tileType;
                        comboCount = 1;
                    }

                    int gemScore = GameManager.Instance.GetGemScore(node.tileType, comboCount);
                    ScoreUIManager.Instance.AddScore(gemScore);
                    GameManager.Instance.SpawnerScore(node.transform.position - new Vector3(0, 0, 0.6f), gemScore);
                    node.ClearItemObject();
                    break;

                case TileType.Ground:
                    node.ClearItemObject();
                    break;
            }
        }
    }


    void UpdateSnakeParts(Node oldHeadNode)
    {
        Node prevNode = oldHeadNode;

        for (int i = 0; i < GameManager.Instance.snakeParts.Count; i++)
        {
            Node temp = GameManager.Instance.snakeParts[i].currentNode;
            temp.SetIsSnake(false);
            GameManager.Instance.snakeParts[i].MoveToNode(prevNode, currentDirection, currentNode);
            prevNode.SetIsSnake(true);
            prevNode = temp;
        }
    }

    Direction? GetDirectionFromNodes(Node target)
    {
        int dx = target.GetIndexX() - currentNode.GetIndexX();
        int dy = target.GetIndexY() - currentNode.GetIndexY();

        if (dx == 1 && dy == 0) return Direction.Right;
        if (dx == -1 && dy == 0) return Direction.Left;
        if (dx == 0 && dy == 1) return Direction.Up;
        if (dx == 0 && dy == -1) return Direction.Down;

        return null;
    }


    private void CheckDirection(Node targetNode)
    {
        Direction? dir = GetDirectionFromNodes(targetNode);

        if (dir.HasValue)
        {
            if (!IsOppositeDirection(currentDirection, dir.Value))
            {
                currentDirection = dir.Value;
                RotatingSnake(currentDirection);
                MoveToNode(targetNode);
            }
        }
    }

    bool IsOppositeDirection(Direction dir1, Direction dir2)
    {
        return (dir1 == Direction.Up && dir2 == Direction.Down) ||
               (dir1 == Direction.Down && dir2 == Direction.Up) ||
               (dir1 == Direction.Left && dir2 == Direction.Right) ||
               (dir1 == Direction.Right && dir2 == Direction.Left);
    }

}


