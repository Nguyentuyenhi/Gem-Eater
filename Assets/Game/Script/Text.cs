using UnityEngine;

public class Text : MonoBehaviour
{
    [SerializeField] private Renderer rd;
    private void Awake()
    {
        rd = GetComponent<Renderer>();
        rd.sortingOrder = 5;
    }
}
