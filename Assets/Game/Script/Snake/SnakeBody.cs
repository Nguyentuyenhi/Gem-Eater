using System.Collections;
using UnityEngine;
using static SnakeBody;

public class SnakeBody : SnakeBase
{
    public enum SnakeType { Body, Tail }
    private Node oldNode;

    [SerializeField] private SnakeType snakeType;
    public void SetCurrentNode(Node node)
    {

        currentNode = node;
    }

    public void MoveToNode(Node node)
    {
        StartCoroutine(MoveSmooth(node));
    }
    protected virtual IEnumerator MoveSmooth(Node newNode)
    {

        while (Vector3.Distance(transform.position, newNode.transform.position) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                newNode.transform.position,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }
        oldNode = currentNode;
        currentNode = newNode;
        currentNode.SetIsSnake(true);
        if (snakeType == SnakeType.Tail)
        {
            oldNode.SetCanDown(true);
            GameManager.Instance.TryCollapseColumn(new Vector2Int(oldNode.indexX, oldNode.indexY ));
        }
        transform.position = newNode.transform.position;

    }


}

