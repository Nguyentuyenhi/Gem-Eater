using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;
using static UnityEditor.VersionControl.Asset;

public class SnakeController : SnakeBase
{
    private bool isDragging = false;
    private bool isMoving = false;

    private List<SnakeBody> snakeParts = new List<SnakeBody>();


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
        snakeParts = Parts;
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
        StartCoroutine(MoveSmooth(newNode));
    }

    IEnumerator MoveSmooth(Node newNode)
    {
        isMoving = true;

        Node oldHeadNode = currentNode;
        UpdateSnakeParts(oldHeadNode);
        while (Vector3.Distance(transform.position, newNode.transform.position) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, newNode.transform.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        currentNode = newNode;
        currentNode.SetIsSnake(true);
        //if (GameManager.Instance.CheckWin())
        //{
        //    Debug.Log("WWWWWIIIIIIINNNNNN");
        //}
        transform.position = newNode.transform.position;
        isMoving = false;
    }

    void UpdateSnakeParts(Node oldHeadNode)
    {
        Node prevNode = oldHeadNode;

        for (int i = 0; i < snakeParts.Count; i++)
        {
            Node temp = snakeParts[i].currentNode;
            temp.SetIsSnake(false);
            snakeParts[i].SetCurrentNode(prevNode);
            prevNode = temp;
        }
    }



    Direction? GetDirectionFromNodes(Node target)
    {
        int dx = target.GetIndexX() - currentNode.GetIndexX();
        int dy = target.GetIndexY() - currentNode.GetIndexY();

        if (dx == 1 && dy == 0) return Direction.Down;
        if (dx == -1 && dy == 0) return Direction.Up;
        if (dx == 0 && dy == 1) return Direction.Right;
        if (dx == 0 && dy == -1) return Direction.Left;

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
