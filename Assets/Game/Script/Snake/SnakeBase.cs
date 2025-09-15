using UnityEngine;

public class SnakeBase : MonoBehaviour
{
    public Node currentNode;
    public enum Direction { Up, Down, Left, Right }
    protected float moveSpeed = 10f;
    protected float distance;
    protected float duration;

    [SerializeField] protected Direction currentDirection = Direction.Up;
    protected virtual void RotatingSnake(Direction dir)
    {
        switch (dir)
        {
            case Direction.Up:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case Direction.Down:
                transform.rotation = Quaternion.Euler(0, 0, 180);
                break;
            case Direction.Left:
                transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case Direction.Right:
                transform.rotation = Quaternion.Euler(0, 0, -90);
                break;
        }
    }
}
