
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    public GameObject bgTilePrefabs;
    public Gem[] gems;
    public Gem[,] allGem;

    void Start()
    {
        allGem = new Gem[width, height];
        Setup();

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
                SpawnGem(gems[gemToUse],new Vector2Int(x,y));
                }
           }
         }
    private void SpawnGem(Gem gemToSpawn,Vector2Int pos)
    {
        Gem gem = Instantiate(gemToSpawn, new Vector3(pos.x, pos.y, 0f), Quaternion.identity);
        gem.transform.parent = transform;
        gem.name = "Gem - " + pos.x + "," + pos.y;
        allGem[pos.x, pos.y] = gem;
        gem.SetUpGem(pos, this);
    }
 
}