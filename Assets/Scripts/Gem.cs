using System.Collections;
using UnityEngine;

public class Gem : MonoBehaviour
{
    //[HideInInspector]
    public Vector2Int posIndex;
    //[HideInInspector]
    public Board board;

    private Vector2 firstTouchPos;
    private Vector2 lastTouchPos;

    private bool mousePressed;
    private float swipeAngle = 0;
    private Gem otherGem;
    public enum gemType { blue,green,purple,red,yellow,bomb,stone }
    public gemType type;
    public bool isMatched;
    private Vector2Int PreviousPos;
    public GameObject destroyEffect;
    public int blastRadius = 2;
    public int scoreValue = 10;

    private void Update()
    {
        if (Vector2.Distance(transform.position, posIndex) > .01f)
        {
            transform.position = Vector2.Lerp(transform.position, posIndex, board.gemSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = new Vector3(posIndex.x, posIndex.y, 0f);
            board.allGem[posIndex.x, posIndex.y] = this;
        }

        if (mousePressed && Input.GetMouseButtonUp(0))
        {
            mousePressed = false;
            if (board.currentState == Board.BoardState.move && board.roundMan.roundTime > 0)
            {
                lastTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                CalculateAngle();
            }
        }
    }
    public void SetUpGem(Vector2Int pos,Board theboard)
    {
        posIndex = pos;
        board = theboard;
    }
    private void OnMouseDown()
    {
        if (board.currentState == Board.BoardState.move && board.roundMan.roundTime > 0)
        {
            firstTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePressed = true;
        }
    }
    private void CalculateAngle()
    {
        swipeAngle = Mathf.Atan2(lastTouchPos.y - firstTouchPos.y, lastTouchPos.x - firstTouchPos.x);
        swipeAngle = swipeAngle * 180 / Mathf.PI;


        if (Vector3.Distance(firstTouchPos, lastTouchPos) > .5f)
        {
            MovePieces();
        }
    }
    private void MovePieces()
    {
        PreviousPos = posIndex;
        otherGem = null;                                            

        if (swipeAngle < 45 && swipeAngle > -45 && posIndex.x < board.width - 1)          // right
        {
            otherGem = board.allGem[posIndex.x + 1, posIndex.y];
            otherGem.posIndex.x--;
            posIndex.x++;
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && posIndex.y < board.height - 1)   // up
        {
            otherGem = board.allGem[posIndex.x, posIndex.y + 1];
            otherGem.posIndex.y--;
            posIndex.y++;
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && posIndex.y > 0)                // down
        {
            otherGem = board.allGem[posIndex.x, posIndex.y - 1];
            otherGem.posIndex.y++;
            posIndex.y--;
        }
        else if ((swipeAngle > 135 || swipeAngle < -135) && posIndex.x > 0)               // left
        {
            otherGem = board.allGem[posIndex.x - 1, posIndex.y];
            otherGem.posIndex.x++;
            posIndex.x--;
        }

        if (otherGem == null) return;                               

        board.allGem[posIndex.x, posIndex.y] = this;
        board.allGem[otherGem.posIndex.x, otherGem.posIndex.y] = otherGem;

        StartCoroutine(CheckMove());
    }

    private IEnumerator CheckMove()
    {
        board.currentState = Board.BoardState.wait;
        yield return new WaitForSeconds(.5f);

        board.matchFinder.FindAllMatch();

        if(otherGem != null)
        {
            if (!isMatched && !otherGem.isMatched)
            {
                otherGem.posIndex = posIndex;
                posIndex = PreviousPos;

                board.allGem[posIndex.x, posIndex.y] = this;
                board.allGem[otherGem.posIndex.x, otherGem.posIndex.y] = otherGem;

                board.currentState = Board.BoardState.move;
            }
            else
            {
                board.DestroyMatches();
            }
        }
    }
}
