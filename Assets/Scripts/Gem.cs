using UnityEngine;

public class Gem : MonoBehaviour
{
    [HideInInspector]
    public Vector2Int posIndex;
    [HideInInspector]
    public Board board;

    private Vector2 firstTouchPos;
    private Vector2 lastTouchPos;

    private bool mousePressed;
    private float swipeAngle = 0;

    private void Update()
    {
        if(mousePressed && Input.GetMouseButtonUp(0))
        {
            lastTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }
    public void SetUpGem(Vector2Int pos,Board theboard)
    {
        posIndex = pos;
        board = theboard;
    }
    private void OnMouseDown()
    {
        firstTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePressed = true;
    }
    private void CalculateAngle()
    {
        swipeAngle = Mathf.Atan2(lastTouchPos.x - firstTouchPos.x, lastTouchPos.y - firstTouchPos.y);
        swipeAngle = swipeAngle * 180 / Mathf.PI;
        Debug.Log(swipeAngle);
    }
}
