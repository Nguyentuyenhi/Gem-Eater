using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.U2D;
using static SnakeBody;

public class SnakeBody : SnakeBase
{
    public enum SnakeType { Body, Tail }
    [SerializeField] private Node oldNode;
    [SerializeField] private Node prevNode;
    [SerializeField] private Node nextNode;
    [SerializeField] private Node headNode;
    [SerializeField] private Node NearestNode;
    [SerializeField] private SnakeType snakeType;

    [SerializeField] private Sprite straightSprite;
    [SerializeField] private Sprite cornerSprite;

    [SerializeField] private SpriteRenderer sr;

    private Vector2 dir;

    private void Awake()
    {
    }
    public void SetCurrentNode(Node node)
    {

        currentNode = node;
    }

    public void MoveToNode(Node node, Direction dir, Node headNodee)
    {
        currentDirection = dir;
        headNode = headNodee;
        MoveSmooth(node);
    }
    protected virtual void MoveSmooth(Node newNode)
    {
        distance = Vector3.Distance(transform.position, newNode.transform.position);
        duration = distance / moveSpeed;

        transform.DOMove(newNode.transform.position, duration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                oldNode = currentNode;
                currentNode = newNode;

                if (snakeType == SnakeType.Tail)
                {
                    //oldNode.SetCanDown(true);
                    Node nearTail = GameManager.Instance.snakeParts[GameManager.Instance.snakeParts.Count - 2].currentNode;
                    RotatePart(currentNode, nearTail);

                    GameManager.Instance.TryCollapseColumn(new Vector2Int(oldNode.indexX, oldNode.indexY));
                }
                transform.position = newNode.transform.position;
                int index = GameManager.Instance.snakeParts.IndexOf(this);
                NearestNode = (index == 0) ? headNode : GameManager.Instance.snakeParts[index - 1].currentNode;
                RotatePart(currentNode, NearestNode);
                UpdateSprite(oldNode, currentNode, NearestNode);

            });
    }

    public void UpdateSprite(Node prev, Node cur, Node next)
    {

            if (prev != null && next != null)
            {
            Vector2 dirPrev = (prev.transform.position - cur.transform.position).normalized;
                Vector2 dirNext = (next.transform.position - cur.transform.position).normalized;

                if (Mathf.Approximately(dirPrev.x, dirNext.x) || Mathf.Approximately(dirPrev.y, dirNext.y))
            {
                sr.sprite = straightSprite;
             
            }
            else
            {
                sr.sprite = cornerSprite;

                if ((dirPrev == Vector2.up && dirNext == Vector2.right) || (dirPrev == Vector2.right && dirNext == Vector2.up))
                    transform.rotation = Quaternion.Euler(0, 0, 90);
                else if ((dirPrev == Vector2.left && dirNext == Vector2.up) || (dirPrev == Vector2.up && dirNext == Vector2.left))
                   transform.rotation = Quaternion.Euler(0, 0, 180);
                else if ((dirPrev == Vector2.down && dirNext == Vector2.left) || (dirPrev == Vector2.left && dirNext == Vector2.down))
                    transform.rotation = Quaternion.Euler(0, 0, -90);
                else if ((dirPrev == Vector2.right && dirNext == Vector2.down) || (dirPrev == Vector2.down && dirNext == Vector2.right))
                    transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }
    protected void RotatePart(Node from, Node to)
    {
        dir = new Vector2(to.indexX - from.indexX, to.indexY - from.indexY);

        if (dir == Vector2.up) transform.rotation = Quaternion.Euler(0, 0, 0);
        else if (dir == Vector2.down) transform.rotation = Quaternion.Euler(0, 0, 180);
        else if (dir == Vector2.left) transform.rotation = Quaternion.Euler(0, 0, 90);
        else if (dir == Vector2.right) transform.rotation = Quaternion.Euler(0, 0, -90);
    }

    private void RotateCorner(Vector2 dirPrev, Vector2 dirNext)
    {
        if ((dirPrev == Vector2.left && dirNext == Vector2.up) ||
            (dirNext == Vector2.left && dirPrev == Vector2.up))
            transform.rotation = Quaternion.Euler(0, 0, 0);

        else if ((dirPrev == Vector2.up && dirNext == Vector2.right) ||
                 (dirNext == Vector2.up && dirPrev == Vector2.right))
            transform.rotation = Quaternion.Euler(0, 0, -90);

        else if ((dirPrev == Vector2.right && dirNext == Vector2.down) ||
                 (dirNext == Vector2.right && dirPrev == Vector2.down))
            transform.rotation = Quaternion.Euler(0, 0, -180);

        else if ((dirPrev == Vector2.down && dirNext == Vector2.left) ||
                 (dirNext == Vector2.down && dirPrev == Vector2.left))
            transform.rotation = Quaternion.Euler(0, 0, 90);
    }

    private void UpdateSnake()
    {
        for (int i = 0; i < GameManager.Instance.snakeParts.Count; i++)
        {
            prevNode = (i == 0) ? headNode : GameManager.Instance.snakeParts[i - 1].currentNode;
            Node cur = GameManager.Instance.snakeParts[i].currentNode;
            nextNode = (i == GameManager.Instance.snakeParts.Count - 1) ? cur : GameManager.Instance.snakeParts[i + 1].currentNode;

            GameManager.Instance.snakeParts[i].RotatePart(cur, nextNode);
        }

    }
    //public void UpdateSprite(Node prev, Node cur, Node next)
    //{
    //    Vector2 dirPrev = (prev.transform.position - cur.transform.position).normalized;
    //    Vector2 dirNext = (next.transform.position - cur.transform.position).normalized;


    //    if (next == cur)
    //    {

    //        if (dirPrev == Vector2.up) transform.rotation = Quaternion.Euler(0, 0, 0);
    //        else if (dirPrev == Vector2.right) transform.rotation = Quaternion.Euler(0, 0, -90);
    //        else if (dirPrev == Vector2.down) transform.rotation = Quaternion.Euler(0, 0, 180);
    //        else if (dirPrev == Vector2.left) transform.rotation = Quaternion.Euler(0, 0, 90);

    //        return;
    //    }

    //    // Nếu prev và next nằm cùng trục → thân thẳng
    //    if (Mathf.Approximately(dirPrev.x, dirNext.x) || Mathf.Approximately(dirPrev.y, dirNext.y))
    //    {
    //        sr.sprite = straightSprite;

    //        if (dirPrev == Vector2.up || dirNext == Vector2.up) transform.rotation = Quaternion.Euler(0, 0, 0);
    //        else if (dirPrev == Vector2.down || dirNext == Vector2.down) transform.rotation = Quaternion.Euler(0, 0, 180);
    //        else if (dirPrev == Vector2.left || dirNext == Vector2.left) transform.rotation = Quaternion.Euler(0, 0, 90);
    //        else if (dirPrev == Vector2.right || dirNext == Vector2.right) transform.rotation = Quaternion.Euler(0, 0, -90);
    //    }
    //    else
    //    {
    //        // Thân cong
    //        sr.sprite = cornerSprite;

    //        // Xác định góc cong dựa vào 4 trường hợp
    //        if ((dirPrev == Vector2.up && dirNext == Vector2.right) || (dirPrev == Vector2.right && dirNext == Vector2.up))
    //            transform.rotation = Quaternion.Euler(0, 0, 0);
    //        else if ((dirPrev == Vector2.right && dirNext == Vector2.down) || (dirPrev == Vector2.down && dirNext == Vector2.right))
    //            transform.rotation = Quaternion.Euler(0, 0, -90);
    //        else if ((dirPrev == Vector2.down && dirNext == Vector2.left) || (dirPrev == Vector2.left && dirNext == Vector2.down))
    //            transform.rotation = Quaternion.Euler(0, 0, 180);
    //        else if ((dirPrev == Vector2.left && dirNext == Vector2.up) || (dirPrev == Vector2.up && dirNext == Vector2.left))
    //            transform.rotation = Quaternion.Euler(0, 0, 90);
    //    }
    //}







}

