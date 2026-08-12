
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject bgTilePrefabs;
    public Gem[] gems;
    public Gem[,] allGem;
    public float gemSpeed;
    public MatchFinder matchFinder;
    public enum BoardState { move,wait};
    public BoardState currentState = BoardState.move;
    public Gem bomb;
    public float bombChance = 2f;
    [HideInInspector]
    public RoundManager roundMan;
    private float bonusMulti;
    public float bonusAmount = .5f;


    private void Awake()
    {
       matchFinder = Object.FindAnyObjectByType<MatchFinder>();
        roundMan = Object.FindAnyObjectByType<RoundManager>();
    }
    void Start()
    {
        allGem = new Gem[width, height];
        Setup();

    }
    private void Update()
    {
        // matchFinder.FindAllMatch();
        if (Input.GetKeyDown(KeyCode.S))
        {
            ShuffleBoard();
        }
    }
    private void Setup()
        { 
           for(int x = 0;x < width;x++)
           {
               for(int y = 0;y < height;y++ )
               {
                Vector2 pos = new Vector2(x, y);
                GameObject bgTile = Instantiate(bgTilePrefabs, pos, Quaternion.identity);
                bgTile.transform.parent = transform;
                bgTile.name = "BGTile - " + x + "," + y;

                int gemToUse = Random.Range(0, gems.Length);
                int chongloi = 0;
                while (MatchAt(new Vector2Int(x,y),gems[gemToUse]) && chongloi < 100)
                {
                    gemToUse = Random.Range(0, gems.Length);
                    chongloi++;
                }

                SpawnGem(gems[gemToUse],new Vector2Int(x,y));
                }
           }
         }
    private void SpawnGem(Gem gemToSpawn,Vector2Int pos)
    {
        if (Random.Range(0f, 100f) < bombChance)
        {
            gemToSpawn = bomb;
        }
            Gem gem = Instantiate(gemToSpawn, new Vector3(pos.x, pos.y + height, 0f), Quaternion.identity);
            gem.transform.parent = transform;
            gem.name = "Gem - " + pos.x + "," + pos.y;
            allGem[pos.x, pos.y] = gem;
            gem.SetUpGem(pos, this);
        
    }

    bool MatchAt(Vector2Int posToCheck,Gem gemToCheck)
    {
        if(posToCheck.x > 1)
        {
            if (allGem[posToCheck.x -1,posToCheck.y].type == gemToCheck.type && allGem[posToCheck.x - 2, posToCheck.y].type == gemToCheck.type)
            {
                return true;
            }
        }
        if(posToCheck.y > 1)
        {
            if (allGem[posToCheck.x,posToCheck.y -1].type == gemToCheck.type && allGem[posToCheck.x, posToCheck.y - 2].type == gemToCheck.type)
            {
                return true;
            }
        }
        return false;
    }

    private void DestroyMatchedAt(Vector2Int pos)
    {
        if (allGem[pos.x ,pos.y] != null)
        {
            if (allGem[pos.x, pos.y].isMatched)
            {
                Instantiate(allGem[pos.x, pos.y].destroyEffect, new Vector2(pos.x, pos.y), Quaternion.identity);
                Destroy(allGem[pos.x, pos.y].gameObject);
                allGem[pos.x, pos.y] = null;
            }
        }
    }

    public void DestroyMatches()
    {
        for(int i = 0; i < matchFinder.currentMatches.Count; i++)
        {
            if (matchFinder.currentMatches[i] != null)
            {
                ScoreCheck(matchFinder.currentMatches[i]); 

                DestroyMatchedAt(matchFinder.currentMatches[i].posIndex);
            }
        }
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
                if (allGem[x,y] == null)
                {
                    nullCounter++;
                }
                else if(nullCounter > 0)
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

        if(matchFinder.currentMatches.Count > 0)
        {
            bonusMulti++;
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
                if (foundGems.Contains(allGem[x,y]))
                {
                    foundGems.Remove(allGem[x,y]);
                }
            }
        }
        foreach(Gem g in foundGems)
        {
            Destroy(g.gameObject);
        }
            }

    public void ShuffleBoard()
    {
        if (currentState != BoardState.wait)
        {
            currentState = BoardState.wait;

            List<Gem> gemsFromBroad = new List<Gem>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    gemsFromBroad.Add(allGem[x, y]);
                    allGem[x, y] = null;

                }
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int gemToUse = Random.Range(0, gemsFromBroad.Count);

                    int chongloi = 0;
                    while(MatchAt(new Vector2Int(x,y), gemsFromBroad[gemToUse]) && chongloi < 100 && gemsFromBroad.Count > 1)
                    {
                        gemToUse = Random.Range(0, gemsFromBroad.Count);
                        chongloi++;
                    }

                    gemsFromBroad[gemToUse].SetUpGem(new Vector2Int(x, y), this);
                    allGem[x, y] = gemsFromBroad[gemToUse];
                    gemsFromBroad.RemoveAt(gemToUse);
                }
            }
            StartCoroutine(FillBoard());
        }


    }
    public void ScoreCheck( Gem gemToCheck)
    {
        roundMan.currentScore += gemToCheck.scoreValue;
        if(bonusMulti > 0)
        {
            float bonusToAdd = gemToCheck.scoreValue * bonusMulti * bonusAmount;
            roundMan.currentScore += Mathf.RoundToInt(bonusToAdd);
        }
    }
 
}