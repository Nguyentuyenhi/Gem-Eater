using DG.Tweening;
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
                currentNode.SetIsSnake(true);

                if (snakeType == SnakeType.Tail)
                {
                    //oldNode.SetCanDown(true);
                    GameManager.Instance.TryCollapseColumn(new Vector2Int(oldNode.indexX, oldNode.indexY));
                }

                transform.position = newNode.transform.position;
            });
    }


}

