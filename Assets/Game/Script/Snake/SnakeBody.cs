using UnityEngine;
using System.Collections;

public class SnakeBody : SnakeBase
{

    public void SetCurrentNode(Node node)
    {
        currentNode = node;
        StartCoroutine(MoveSmooth(node));
    }

    IEnumerator MoveSmooth(Node newNode)
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
        currentNode.SetIsSnake(true);
        transform.position = newNode.transform.position;
    }

}
