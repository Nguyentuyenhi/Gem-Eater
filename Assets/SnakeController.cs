using UnityEngine;
using System.Collections;

public class SnakeController : MonoBehaviour
{
    public Node currentNode;
    public bool isDragging = false;

    public float moveSpeed = 5f;
    private bool isMoving = false; // cờ kiểm tra rắn đang di chuyển


    public enum Direction { Up, Down, Left, Right }

    [SerializeField]private Direction currentDirection = Direction.Right;
    private void Update()
    {
        if (isDragging && !isMoving) // chỉ nhận input khi không di chuyển
        {
            HandleMouseDrag();
        }
    }

    public void SetCurrentNode(Node node)
    {
        currentNode = node;
        transform.position = node.transform.position;
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

        while (Vector3.Distance(transform.position, newNode.transform.position) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, newNode.transform.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        currentNode = newNode;
        currentNode.SetIsSnake(true);
        transform.position = newNode.transform.position;

        isMoving = false; 
    }

    Direction? GetDirectionFromNodes( Node target)
    {
        int dx = target.GetIndexX() - currentNode.GetIndexX();
        int dy = target.GetIndexY() - currentNode.GetIndexY();

        if (dx == 1 && dy == 0) return Direction.Right;
        if (dx == -1 && dy == 0) return Direction.Left;
        if (dx == 0 && dy == 1) return Direction.Up;
        if (dx == 0 && dy == -1) return Direction.Down;

        return null;
    }

    public void CheckDirection(Node targetNode)
    {
        Direction? dir = GetDirectionFromNodes(targetNode);

        if (dir.HasValue)
        {
            if (!IsOppositeDirection(currentDirection, dir.Value))
            {
                currentDirection = dir.Value;
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
