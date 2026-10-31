using UnityEngine;
using UnityEngine.SceneManagement;

public class RoundManager : MonoBehaviour
{
    public float roundTime = 60f;
    private UIManager uiMan;
    private bool endingRound = false;
    private bool roundFinished = false;
    private Board board;

    public int currentScore;
    [HideInInspector]
    public float displayScore;
    public float scoreSpeed;
    public int scoreTarget1, scoreTarget2, scoreTarget3;

    [Header("Mục tiêu level (tùy chọn)")]
    public bool useObjective = false;
    public Gem.gemType objectiveGemType = Gem.gemType.red;
    public int objectiveAmount = 20;
    [HideInInspector]
    public int objectiveProgress;

    [Header("Giới hạn lượt đi (0 = không giới hạn)")]
    public int maxMoves = 0;
    [HideInInspector]
    public int movesLeft;
    public bool IsPlaying
    {
        get { return roundTime > 0f && !endingRound && !roundFinished; }
    }

    private void Awake()
    {
        uiMan = Object.FindAnyObjectByType<UIManager>();
        board = Object.FindAnyObjectByType<Board>();
        movesLeft = maxMoves;
    }

    private void Start()
    {
        RefreshObjectiveUI();
    }

    void Update()
    {
        // thời gian chỉ chạy khi bảng đang ở trạng thái chờ người chơi; đang nổ/combo thì dừng
        if (roundTime > 0 && !endingRound && !roundFinished && board.currentState == Board.BoardState.move)
        {
            roundTime -= Time.deltaTime;
            if (roundTime <= 0)
            {
                roundTime = 0;
                endingRound = true;
            }
        }

        if (endingRound && board.currentState == Board.BoardState.move)
        {
            endingRound = false;
            roundFinished = true;
            WinCheck();
        }


        uiMan.timeText.text = roundTime.ToString("0.0") + "s";

        displayScore = Mathf.Lerp(displayScore, currentScore, scoreSpeed * Time.deltaTime);
        uiMan.scoreText.text = displayScore.ToString("0");
    }


    public void CollectGem(Gem.gemType type)
    {
        if (!useObjective || type != objectiveGemType)
        {
            return;
        }
        objectiveProgress++;
        RefreshObjectiveUI();
    }


    public void UseMove()
    {
        if (maxMoves <= 0 || roundFinished)
        {
            return;
        }
        movesLeft--;
        RefreshObjectiveUI();

        if (movesLeft <= 0)
        {
            endingRound = true;
        }
    }

    private void RefreshObjectiveUI()
    {
        if (uiMan == null)
        {
            return;
        }

        if (uiMan.objectiveText != null)
        {
            if (useObjective)
            {
                string progress = Mathf.Min(objectiveProgress, objectiveAmount) + "/" + objectiveAmount;
                if (objectiveProgress >= objectiveAmount)
                {
                    progress += " (Hoàn thành!)";
                }
                uiMan.objectiveText.text = "Thu thập gem " + GemNameVN(objectiveGemType) + ": " + progress;
            }
            else
            {
                uiMan.objectiveText.text = "";
            }
        }

        if (uiMan.movesText != null)
        {
            uiMan.movesText.text = maxMoves > 0 ? "Lượt đi: " + movesLeft : "";
        }
    }

    private string GemNameVN(Gem.gemType type)
    {
        switch (type)
        {
            case Gem.gemType.blue: return "xanh dương";
            case Gem.gemType.green: return "xanh lá";
            case Gem.gemType.purple: return "tím";
            case Gem.gemType.red: return "đỏ";
            case Gem.gemType.yellow: return "vàng";
            default: return type.ToString();
        }
    }

    private void WinCheck()
    {
        uiMan.RoundOverScreen.SetActive(true);
        uiMan.winScore.text = currentScore.ToString();

        bool objectiveDone = !useObjective || objectiveProgress >= objectiveAmount;

        int stars = 0;
        if (objectiveDone)
        {
            if (currentScore >= scoreTarget3)
            {
                stars = 3;
            }
            else if (currentScore >= scoreTarget2)
            {
                stars = 2;
            }
            else if (currentScore >= scoreTarget1)
            {
                stars = 1;
            }
            if (useObjective && stars < 1)
            {
                stars = 1;
            }
        }

        if (stars == 3)
        {
            uiMan.winText.text = "Chúc Mừng!  Bạn được 3 sao !!";
            uiMan.winStars3.SetActive(true);
        }
        else if (stars == 2)
        {
            uiMan.winText.text = "Chúc Mừng Bạn được 2 sao !!";
            uiMan.winStars2.SetActive(true);
        }
        else if (stars == 1)
        {
            uiMan.winText.text = "Chúc Mừng Bạn được 1 sao !!";
            uiMan.winStars1.SetActive(true);
        }
        else if (!objectiveDone)
        {
            uiMan.winText.text = "Chưa hoàn thành mục tiêu!! Thử lại ?";
        }
        else
        {
            uiMan.winText.text = "Bạn không nhận được sao nào cả!! Thử lại ?";
        }

        string levelName = SceneManager.GetActiveScene().name;
        if (stars >= 1) PlayerPrefs.SetInt(levelName + "_Star1", 1);
        if (stars >= 2) PlayerPrefs.SetInt(levelName + "_Star2", 2);
        if (stars >= 3) PlayerPrefs.SetInt(levelName + "_Star3", 3);
        PlayerPrefs.Save();

        SFXManager.instance.PlayRoundOverSound();
    }
}