using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Gem;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject bgTilePrefabs;
    public Gem[] gems;
    public Gem[,] allGem;
    public float gemSpeed;
    public MatchFinder matchFinder;
    public enum BoardState { move, wait };
    public BoardState currentState = BoardState.move;
    public Gem bomb;
    [HideInInspector]
    public RoundManager roundMan;
    private float bonusMulti;
    public float bonusAmount = .5f;
    public Gem heart;
    public int maxShuffle = 3;
    private int shuffleCount = 0;

    private UIManager uiMan;


    private BoardLayout boardLayout;
    private Gem[,] layoutStore;


    private void Awake()
    {
        matchFinder = Object.FindAnyObjectByType<MatchFinder>();
        roundMan = Object.FindAnyObjectByType<RoundManager>();
        uiMan = Object.FindAnyObjectByType<UIManager>();
        boardLayout = GetComponent<BoardLayout>();
    }
    void Start()
    {
        allGem = new Gem[width, height];

        layoutStore = new Gem[width, height];
        Setup();

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            ShuffleBoard();
        }
    }
    private void Setup()
    {
        if (boardLayout != null)
        {
            layoutStore = boardLayout.GetLayout(width, height);
        }


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 pos = new Vector2(x, y);
                GameObject bgTile = Instantiate(bgTilePrefabs, pos, Quaternion.identity);
                bgTile.transform.parent = transform;
                bgTile.name = "BGTile - " + x + "," + y;

                if (layoutStore[x, y] != null)
                {
                    SpawnGem(layoutStore[x, y], new Vector2Int(x, y));
                }
                else
                {
                    int gemToUse = Random.Range(0, gems.Length);
                    int chongloi = 0;
                    while (MatchAt(new Vector2Int(x, y), gems[gemToUse]) && chongloi < 100)
                    {
                        gemToUse = Random.Range(0, gems.Length);
                        chongloi++;
                    }

                    SpawnGem(gems[gemToUse], new Vector2Int(x, y));
                }
            }
        }
    }
    private void SpawnGem(Gem gemToSpawn, Vector2Int pos)
    {
        // ô nằm dưới stone thì gem mới chỉ rơi từ ngay trên ô đó, không xuyên qua stone
        float startY = StoneAbove(pos.x, pos.y) ? pos.y + 1f : pos.y + height;
        Gem gem = Instantiate(gemToSpawn, new Vector3(pos.x, startY, 0f), Quaternion.identity);
        gem.transform.parent = transform;
        gem.name = "Gem - " + pos.x + "," + pos.y;
        allGem[pos.x, pos.y] = gem;
        gem.SetUpGem(pos, this);

    }


    private void SpawnPendingSpecials()
    {
        for (int i = 0; i < matchFinder.pendingSpecials.Count; i++)
        {
            MatchFinder.SpecialSpawn spawn = matchFinder.pendingSpecials[i];


            Gem prefab = (spawn.type == gemType.heart) ? heart : bomb;
            if (prefab == null)
            {
                continue;
            }
            if (allGem[spawn.pos.x, spawn.pos.y] != null)
            {
                continue;
            }

            Gem gem = Instantiate(prefab, new Vector3(spawn.pos.x, spawn.pos.y, 0f), Quaternion.identity);
            gem.transform.parent = transform;
            gem.name = "Special - " + spawn.pos.x + "," + spawn.pos.y;
            allGem[spawn.pos.x, spawn.pos.y] = gem;
            gem.SetUpGem(spawn.pos, this);
        }
        matchFinder.pendingSpecials.Clear();
    }

    private bool StoneAbove(int x, int y)
    {
        for (int yy = y + 1; yy < height; yy++)
        {
            if (allGem[x, yy] != null && allGem[x, yy].type == gemType.stone)
            {
                return true;
            }
        }
        return false;
    }

    bool MatchAt(Vector2Int posToCheck, Gem gemToCheck)
    {
        if (posToCheck.x > 1)
        {
            if (allGem[posToCheck.x - 1, posToCheck.y].type == gemToCheck.type && allGem[posToCheck.x - 2, posToCheck.y].type == gemToCheck.type)
            {
                return true;
            }
        }
        if (posToCheck.y > 1)
        {
            if (allGem[posToCheck.x, posToCheck.y - 1].type == gemToCheck.type && allGem[posToCheck.x, posToCheck.y - 2].type == gemToCheck.type)
            {
                return true;
            }
        }
        return false;
    }

    private void DestroyMatchedAt(Vector2Int pos)
    {
        if (allGem[pos.x, pos.y] != null)
        {
            if (allGem[pos.x, pos.y].isMatched)
            {
                if (allGem[pos.x, pos.y].type == Gem.gemType.bomb)
                {
                    SFXManager.instance.PlayExplode();
                }
                else if (allGem[pos.x, pos.y].type == Gem.gemType.stone)
                {
                    SFXManager.instance.PlayStoneBreak();
                }
                else if (allGem[pos.x, pos.y].type == Gem.gemType.heart)
                {
                    SFXManager.instance.PlayHeartBreak();
                }
                else
                {
                    SFXManager.instance.PlayGemBreak();
                }
                Instantiate(allGem[pos.x, pos.y].destroyEffect, new Vector2(pos.x, pos.y), Quaternion.identity);
                Destroy(allGem[pos.x, pos.y].gameObject);
                allGem[pos.x, pos.y] = null;
            }
        }
    }

    public void DestroyMatches()
    {
        for (int i = 0; i < matchFinder.currentMatches.Count; i++)
        {
            if (matchFinder.currentMatches[i] != null)
            {
                ScoreCheck(matchFinder.currentMatches[i]);
                roundMan.CollectGem(matchFinder.currentMatches[i].type);

                DestroyMatchedAt(matchFinder.currentMatches[i].posIndex);
            }
        }

        SpawnPendingSpecials();

        StartCoroutine(DecreaseRow());
    }

    private IEnumerator DecreaseRow()
    {
        yield return new WaitForSeconds(.2f);

        int nullCounter = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allGem[x, y] != null && allGem[x, y].type == gemType.stone)
                {
                    // stone đứng yên: gem phía trên chỉ rơi tới sát stone, không xuyên qua
                    nullCounter = 0;
                }
                else if (allGem[x, y] == null)
                {
                    nullCounter++;
                }
                else if (nullCounter > 0)
                {
                    allGem[x, y].posIndex.y -= nullCounter;
                    allGem[x, y - nullCounter] = allGem[x, y];
                    allGem[x, y] = null;
                }

            }
            nullCounter = 0;
        }
        StartCoroutine(FillBoard());
    }
    private IEnumerator FillBoard()
    {
        yield return new WaitForSeconds(.5f);
        RefillBoard();

        yield return new WaitForSeconds(.5f);

        matchFinder.FindAllMatch();

        if (matchFinder.currentMatches.Count > 0)
        {
            bonusMulti++;


            if (uiMan != null)
            {
                uiMan.ShowCombo(Mathf.RoundToInt(bonusMulti) + 1);
            }

            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
        else
        {
            yield return new WaitForSeconds(.5f);
            currentState = BoardState.move;
            bonusMulti = 0f;
        }


    }
    private void RefillBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allGem[x, y] == null)
                {
                    int gemToUse = Random.Range(0, gems.Length);
                    SpawnGem(gems[gemToUse], new Vector2Int(x, y));
                }
            }
        }
        CheckMisplacedGem();
    }

    private void CheckMisplacedGem()
    {
        List<Gem> foundGems = new List<Gem>();

        foundGems.AddRange(FindObjectsByType<Gem>(FindObjectsInactive.Exclude));

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (foundGems.Contains(allGem[x, y]))
                {
                    foundGems.Remove(allGem[x, y]);
                }
            }
        }
        foreach (Gem g in foundGems)
        {
            Destroy(g.gameObject);
        }
    }

    public void ShuffleBoard()
    {
        if (currentState != BoardState.wait)
        {
            if (shuffleCount >= maxShuffle)
            {
                return;
            }
            shuffleCount++;
            currentState = BoardState.wait;

            List<Gem> gemsFromBroad = new List<Gem>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (allGem[x, y] != null && allGem[x, y].type != gemType.stone)
                    {
                        gemsFromBroad.Add(allGem[x, y]);
                        allGem[x, y] = null;
                    }
                }
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (allGem[x, y] != null && allGem[x, y].type == gemType.stone)
                    {
                        continue;
                    }

                    if (gemsFromBroad.Count == 0)
                    {
                        continue;
                    }

                    int gemToUse = Random.Range(0, gemsFromBroad.Count);

                    int chongloi = 0;

                    while (MatchAt(
                        new Vector2Int(x, y), gemsFromBroad[gemToUse]) && chongloi < 100 && gemsFromBroad.Count > 1)
                    {
                        gemToUse = Random.Range(0, gemsFromBroad.Count);
                        chongloi++;
                    }

                    Gem selectedGem = gemsFromBroad[gemToUse];

                    selectedGem.SetUpGem(new Vector2Int(x, y), this);
                    selectedGem.transform.position = new Vector3(x, y, 0f);

                    allGem[x, y] = selectedGem;

                    gemsFromBroad.RemoveAt(gemToUse);
                }
            }
            currentState = BoardState.move;
        }
    }
    public void ScoreCheck(Gem gemToCheck)
    {
        roundMan.currentScore += gemToCheck.scoreValue;
        if (bonusMulti > 0)
        {
            float bonusToAdd = gemToCheck.scoreValue * bonusMulti * bonusAmount;
            roundMan.currentScore += Mathf.RoundToInt(bonusToAdd);
        }
    }

}