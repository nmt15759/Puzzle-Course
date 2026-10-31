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

    public enum gemType { blue, green, purple, red, yellow, bomb, stone, heart }
    public gemType type;


    public bool IsNormal { get { return type <= gemType.yellow; } }

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
            if (board.currentState == Board.BoardState.move && board.roundMan.IsPlaying)
            {
                lastTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                CalculateAngle();
            }
        }
    }
    public void SetUpGem(Vector2Int pos, Board theboard)
    {
        posIndex = pos;
        board = theboard;
    }
    private void OnMouseDown()
    {
        if (type == gemType.stone)
        {
            mousePressed = false;
            return;
        }

        if (board.currentState == Board.BoardState.move && board.roundMan.IsPlaying)
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
        if (type == gemType.stone)
        {
            mousePressed = false;
            return;
        }

        Vector2Int direction = Vector2Int.zero;

        if (swipeAngle < 45 && swipeAngle > -45 && posIndex.x < board.width - 1)          // right
        {
            direction = Vector2Int.right;
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && posIndex.y < board.height - 1)   // up
        {
            direction = Vector2Int.up;
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && posIndex.y > 0)                // down
        {
            direction = Vector2Int.down;
        }
        else if ((swipeAngle > 135 || swipeAngle < -135) && posIndex.x > 0)               // left
        {
            direction = Vector2Int.left;
        }

        if (direction == Vector2Int.zero) return;

        Vector2Int targetPos = posIndex + direction;
        Gem targetGem = board.allGem[targetPos.x, targetPos.y];

        // Stone đứng yên: không bị hoán đổi, gem khác cũng không thể vào ô của stone.
        // (ô trống cũng không hoán đổi được)
        if (targetGem == null || targetGem.type == gemType.stone)
        {
            return;
        }

        PreviousPos = posIndex;
        otherGem = targetGem;

        otherGem.posIndex = posIndex;
        posIndex = targetPos;

        board.allGem[posIndex.x, posIndex.y] = this;
        board.allGem[otherGem.posIndex.x, otherGem.posIndex.y] = otherGem;

        StartCoroutine(CheckMove());
    }

    private IEnumerator CheckMove()
    {
        board.currentState = Board.BoardState.wait;
        yield return new WaitForSeconds(.5f);

        board.matchFinder.FindAllMatch();

        if (otherGem != null)
        {
            if (type == gemType.heart && otherGem.IsNormal)
            {
                board.matchFinder.MarkColorClear(this, otherGem.type);
            }
            else if (otherGem.type == gemType.heart && IsNormal)
            {
                board.matchFinder.MarkColorClear(otherGem, type);
            }

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

                board.roundMan.UseMove();
                board.DestroyMatches();
            }
        }
    }
}