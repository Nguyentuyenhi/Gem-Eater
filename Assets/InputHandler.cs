using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance;
    public Camera mainCam;

    public Vector2Int requestedDirection { get; private set; } = Vector2Int.zero;
    private bool isHolding = false;

    void Start()
    {
        if (mainCam == null) mainCam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isHolding = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            isHolding = false;
            requestedDirection = Vector2Int.zero;
        }

        if (isHolding)
        {
            Vector3 mouseWorld = mainCam.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    public Vector3 GetMouseWorldPosition()
    {
        if (mainCam == null) mainCam = Camera.main;
        return mainCam.ScreenToWorldPoint(Input.mousePosition);
    }
}
