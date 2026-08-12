using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MatchFinder : MonoBehaviour
{
    private Board board;
    public List<Gem> currentMatches = new List<Gem>();
   
    private void Awake()
    {
        board = Object.FindAnyObjectByType<Board>();

    }
    public void FindAllMatch()
    {
        currentMatches.Clear();
        for (int x = 0; x < board.width; x++)
        {
            for (int y = 0; y < board.height; y++)
            {
                Gem currentGem = board.allGem[x, y];
                if(currentGem != null)
                {
                    if(x > 0 && x < board.width - 1)
                    {
                        Gem leftGem = board.allGem[x - 1, y];
                        Gem rightGem = board.allGem[x + 1, y];
                        if(leftGem != null && rightGem != null)
                        {
                            if(leftGem.type == currentGem.type && rightGem.type == currentGem.type)
                            {
                                currentGem.isMatched = true;
                                leftGem.isMatched = true;
                                rightGem.isMatched = true;

                                currentMatches.Add(currentGem);
                                currentMatches.Add(leftGem);
                                currentMatches.Add(rightGem);
                            }
                        }
                    }

                    if (y > 0 && y < board.height - 1)
                    {
                        Gem aboveGem = board.allGem[x, y + 1 ];
                        Gem belowGem = board.allGem[x, y - 1];
                        if (aboveGem != null && belowGem != null)
                        {
                            if (aboveGem.type == currentGem.type && belowGem.type == currentGem.type)
                            {
                                currentGem.isMatched = true;
                                aboveGem.isMatched = true;
                                belowGem.isMatched = true;

                                currentMatches.Add(currentGem);
                                currentMatches.Add(aboveGem);
                                currentMatches.Add(belowGem);
                            }
                        }
                    }
                }
            }
        }
        if(currentMatches.Count > 0)
        {
            currentMatches = currentMatches.Distinct().ToList();
        }
        Checkforbomb();
    }
    public void Checkforbomb()
    {
        for(int i = 0;i < currentMatches.Count; i++)
        {
            Gem gem = currentMatches[i];
            int x = gem.posIndex.x;
            int y = gem.posIndex.y;

            if(gem.posIndex.x > 0)
            {
                if (board.allGem[x-1,y] != null)
                {
                    if (board.allGem[x - 1, y].type == Gem.gemType.bomb )
                    {
                        MarkBombArea(new Vector2Int(x-1,y), board.allGem[x-1,y]);
                    }
                }
            }

            if (gem.posIndex.x < board.width -1)
            {
                if (board.allGem[x + 1, y] != null)
                {
                    if (board.allGem[x + 1, y].type == Gem.gemType.bomb)
                    {
                        MarkBombArea(new Vector2Int(x + 1, y), board.allGem[x + 1, y]);
                    }
                }
            }

            if (gem.posIndex.y > 0)
            {
                if (board.allGem[x, y - 1] != null)
                {
                    if (board.allGem[x, y -1].type == Gem.gemType.bomb)
                    {
                        MarkBombArea(new Vector2Int(x, y - 1 ), board.allGem[x , y - 1]);
                    }
                }
            }

            if (gem.posIndex.y < board.height -1)
            {
                if (board.allGem[x , y + 1 ] != null)
                {
                    if (board.allGem[x, y + 1].type == Gem.gemType.bomb)
                    {
                        MarkBombArea(new Vector2Int(x, y + 1), board.allGem[x, y + 1]);
                    }
                }
            }
        }
    }
    public void MarkBombArea(Vector2Int bombPos,Gem thebomb)
    {
        for(int x = bombPos.x - thebomb.blastRadius;x <= bombPos.x + thebomb.blastRadius; x++)
        {
            for(int y = bombPos.y - thebomb.blastRadius;y <= bombPos.y + thebomb.blastRadius; y++)
            {
                if(x >= 0 && x<board.width && y >=0 && y < board.height)
                {
                    if (board.allGem[x,y] != null)
                    {
                        board.allGem[x, y].isMatched = true;
                        currentMatches.Add(board.allGem[x, y]);
                    }
                }
            }
        }
        currentMatches = currentMatches.Distinct().ToList();
    }
}
