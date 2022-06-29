using UnityEngine;
using UnityEngine.Tilemaps;

public class Main : MonoBehaviour
{
    public Camera mainCamera;
    public Tilemap lifeTileMap;
    public Tile life, fadingLife;

    public float spawnLifeCheckBar = 0.75f;
    public float timeBetweenGenerations = 0.1f;

    private int areaX = 200;
    private int areaY = 120;
    public int xOffset, yOffset;

    public float frequency = 100;
    public float smoothness = 10f;

    private bool lifeActive;

    public float[,] currentLifeMap;    // This array if fully intialized later for C# rules
    public float[,] nextLifeMap;

    public void Start()
    {
        currentLifeMap  = new float[areaX, areaY];
        nextLifeMap     = new float[areaX, areaY];

        mainCamera.transform.position = new Vector3(areaX/2, areaY/2, -10);

        beginLife();
    }

    public void Update()
    {
        if (Input.GetKeyDown("space")) 
        {
            resetLife();
        }
    }

    public void nextGeneration()
    {
        if (lifeActive == true)
        {
            nextLifeMap = (new float[areaX, areaY]);

            for (int x = 1; x <= areaX - 2; x++)
            {
                for (int y = 1; y <= areaY - 2; y++)
                {
                    Vector3Int currentTile = new Vector3Int(x, y, 0);

                    
                    if (lifeTileMap.GetTile(currentTile) == fadingLife)
                    {
                        lifeTileMap.SetTile(currentTile, null);
                    }

                    if (currentLifeMap[x, y] == 1f)
                    {
                        // Solitude Check (If a "living" cell is only surronded by <= 1 "living" neighbor it dies)
                        if ((currentLifeMap[x - 1, y + 1] + currentLifeMap[x, y + 1] + currentLifeMap[x + 1, y + 1] +
                             currentLifeMap[x - 1, y    ] + currentLifeMap[x, y    ] + currentLifeMap[x + 1, y    ] +
                             currentLifeMap[x - 1, y - 1] + currentLifeMap[x, y - 1] + currentLifeMap[x + 1, y - 1]) 
                             <= 2)
                        {
                            nextLifeMap[x, y] = 0f;
                            lifeTileMap.SetTile(currentTile, fadingLife);
                        }

                        // Overpopulation Check (If a "living" cell is surroned by >= 4 "living" neighbors it dies)
                        else if ((currentLifeMap[x - 1, y + 1] + currentLifeMap[x, y + 1] + currentLifeMap[x + 1, y + 1] +
                             currentLifeMap[x - 1, y] + currentLifeMap[x, y] + currentLifeMap[x + 1, y] +
                             currentLifeMap[x - 1, y - 1] + currentLifeMap[x, y - 1] + currentLifeMap[x + 1, y - 1])
                             >= 5)
                        {
                            nextLifeMap[x, y] = 0f;
                            lifeTileMap.SetTile(currentTile, fadingLife);
                        }

                        // Ideal Conditions Check (If a "living" cell is surronded by 2 or 3 "living" neighbors it lives
                        else
                        {
                            nextLifeMap[x, y] = 1f;

                            lifeTileMap.SetTile(currentTile, life);
                        }
                    }

                    

                    else
                    {
                        // Ideal Living Check (If a "dead" cell is surronded by 3 "living" neighbors it lives
                        if ((currentLifeMap[x - 1, y + 1] + currentLifeMap[x, y + 1] + currentLifeMap[x + 1, y + 1] +
                             currentLifeMap[x - 1, y] + currentLifeMap[x, y] + currentLifeMap[x + 1, y] +
                             currentLifeMap[x - 1, y - 1] + currentLifeMap[x, y - 1] + currentLifeMap[x + 1, y - 1])
                             == 3)
                        {
                            nextLifeMap[x, y] = 1f;

                            lifeTileMap.SetTile(currentTile, life);
                        }
                    }
                }
            }

            currentLifeMap = nextLifeMap;
        }
    }

    public void beginLife()
    {
        xOffset = Mathf.RoundToInt(Random.Range(0f, 99999f));
        yOffset = Mathf.RoundToInt(Random.Range(0f, 99999f));

        for (int x = 0; x <= areaX - 1; x++)
        {
            for (int y = 0; y <= areaY - 1; y++)
            {
                float spawnLifeCheck = frequency * Mathf.PerlinNoise((x + xOffset) / smoothness, (y + yOffset) / smoothness) / (frequency / 2);

                if (spawnLifeCheck >= spawnLifeCheckBar)
                {
                    currentLifeMap[x, y] = 1f;

                    lifeTileMap.SetTile(new Vector3Int(x, y, 0), life);
                }

            }

        }

        lifeActive = true;

        InvokeRepeating("nextGeneration", timeBetweenGenerations, timeBetweenGenerations);
    }

    public void resetLife()
    {
        CancelInvoke();

        lifeActive = false;

        currentLifeMap = new float[areaX, areaY];
        lifeTileMap.ClearAllTiles();

        beginLife();
    }
}

/*
 *                     if (lifeTileMap.HasTile(currentTile))
                    {
                        lifeTileMap.SetTile(currentTile, null);
                    }
                    else
                    {
                        lifeTileMap.SetTile(currentTile, life);
                    }
 */


/*
 * else if (currentLifeMap[x, y] == 0.5f)
                    {
                        if ((currentLifeMap[x - 1, y + 1] + currentLifeMap[x, y + 1] + currentLifeMap[x + 1, y + 1] +
                             currentLifeMap[x - 1, y] + currentLifeMap[x, y] + currentLifeMap[x + 1, y] +
                             currentLifeMap[x - 1, y - 1] + currentLifeMap[x, y - 1] + currentLifeMap[x + 1, y - 1])
                             <= 4 &&
                             (currentLifeMap[x - 1, y + 1] + currentLifeMap[x, y + 1] + currentLifeMap[x + 1, y + 1] +
                             currentLifeMap[x - 1, y] + currentLifeMap[x, y] + currentLifeMap[x + 1, y] +
                             currentLifeMap[x - 1, y - 1] + currentLifeMap[x, y - 1] + currentLifeMap[x + 1, y - 1])
                             >= 3)
                        {
                            nextLifeMap[x, y] = 1f;
                            lifeTileMap.SetTile(currentTile, life);
                        }

                        else
                        {
                            nextLifeMap[x, y] = 0f;
                            lifeTileMap.SetTile(currentTile, null);
                        }
                    }
 */