using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public float roundTime = 60f;
    private UIManager uiMan;
    private bool endingRound = false;
    private Board board;
  
    private void Awake()
    {
        uiMan = Object.FindAnyObjectByType<UIManager>();
        board = Object.FindAnyObjectByType<Board>();

    }


    void Update()
    {
        if(roundTime > 0)
        {
            roundTime -= Time.deltaTime;
            if(roundTime <= 0)
            {
                roundTime = 0;
                endingRound = true;
            }
        }
        if(endingRound && board.currentState == Board.BoardState.move)
        {
            WinCheck();
            endingRound = false;
        }

        uiMan.timeText.text = roundTime.ToString("0.0") + "s"; 
    }
    private void WinCheck()
    {
        uiMan.RoundOverScreen.SetActive(true);
    }
}
