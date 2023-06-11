using System.Collections;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public enum GamePhase { PickingBugs, Battling, NoGame }

    public static GamePhase gamePhase;
    public GameObject gridSpacePrefab;
    public int width;
    public int height;
    public Bug[,] playingBugs;
    private GridSpace[,] gridSpaces;
    public Bug[] bugs;

    public float spacing = 0.3f;

    private void Start()
    {
        gridSpaces = new GridSpace[width, height];
        playingBugs = new Bug[width, height];
        CreateGrid();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && gamePhase == GamePhase.PickingBugs)
        {
            gamePhase = GamePhase.Battling;
            AddEnemies();
            RegisterBugs();
            StartBattle();
        }
    }

    private void CreateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Calculate the position for the new grid space
                Vector3 position = new(transform.position.x + x * spacing, transform.position.y + y * spacing, 0);

                // Instantiate a new grid space at this position
                GameObject newGridSpace = Instantiate(gridSpacePrefab, position, Quaternion.identity);

                // Make the new grid space a child of this grid object
                newGridSpace.transform.parent = transform;

                gridSpaces[x, y] = newGridSpace.GetComponent<GridSpace>();
                if (x >= width/2)
                {
                    gridSpaces[x, y].canAddBugs = false;
                }
            }
        }
    }

    public void StartBattle()
    {
        StartCoroutine(Battle());
    }


    private void AddEnemies()
    {
        double amount = Mathf.Floor(width / 2); 
        for (int i = width-1; i > width-amount-1; i--)
        {
            for (int j = 0; j < height; j++)
            {
                var pos = gridSpaces[i, j].transform.position;
                var bugIndex = Random.Range(0, bugs.Length);
                var bug = Instantiate(bugs[bugIndex], pos, Quaternion.identity);
                bug.moving = false;
                playingBugs[i, j] = bug;
                bug.IsFriendly = false;
                gridSpaces[i, j].friendlySquare = false;
                bug.GetComponent<SpriteRenderer>().color = Color.red;
                bug.GetComponent<SpriteRenderer>().flipX = true;
            }
        }
    }


    private void RegisterBugs()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (gridSpaces[i, j].currBug != null)
                {
                    playingBugs[i, j] = gridSpaces[i, j].currBug;
                    playingBugs[i, j].moving = false;
                    playingBugs[i, j].isFighting = true; // TODO change into bug state?
                    var sprite = playingBugs[i, j].GetComponent<SpriteRenderer>();
                    sprite.flipX = i >= width / 2;
                        
                }
            }
        }
    }
    private bool CheckDead(int i, int j)
    {
        if (playingBugs[i, j] && playingBugs[i, j].Health <= 0)
        {
            Destroy(playingBugs[i, j].gameObject);
            playingBugs[i, j] = null;
            gridSpaces[i, j].currBug = null;
            return true;
        }
        return false;
    }

    private bool IsFinished()
    {
        var allFriendly = true;
        var allEnemies = true;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (playingBugs[i, j] != null)
                {
                    if (playingBugs[i, j].IsFriendly)
                    {
                        allEnemies = false;
                    } else if (!playingBugs[i, j].IsFriendly)
                    {
                        allFriendly = false;
                    }
                    if (!allEnemies && !allFriendly) return false;
                }
            }
        }
        return true;
    }

    private void BugFight()
    {

    }

    private IEnumerator Battle()
    {
        while (!IsFinished())
        {
            for (int x = width - 1; x >= 0; x--)
            {
                // Go Right to Left for the friendly bugs
                for (int y = 0; y < height; y++)
                {
                    Bug currentBug = playingBugs[x, y];
                    if (currentBug != null && currentBug.IsFriendly)
                    {
                        // Check if the bug can move to the next space
                        if (x + 1 < width && playingBugs[x + 1, y] == null)
                        {
                            currentBug.Move(gridSpaces[x + 1, y]);
                            gridSpaces[x + 1, y].currBug = currentBug;
                            playingBugs[x + 1, y] = currentBug;
                            playingBugs[x, y] = null;
                            gridSpaces[x, y].currBug = null;
                        }
                        // If the next space is taken by an enemy, fight
                        else if (x + 1 < width && playingBugs[x + 1, y] != null && playingBugs[x + 1, y].IsFriendly != currentBug.IsFriendly)
                        {
                            currentBug.Attack(playingBugs[x + 1, y]);
                            CheckDead(x, y);
                        }
                    }
                }
            }
            for (int x = 0; x < width; x++)
            { 
                    // Go Left to Right for the enemy bugs
                    for (int y = 0; y < height; y++)
                {
                    Bug currentBug = playingBugs[x, y];
                    if (currentBug != null && !currentBug.IsFriendly)
                    {
                        // Check if the bug can move to the next space
                        if (x - 1 >= 0 && playingBugs[x - 1, y] == null)
                        {
                            currentBug.Move(gridSpaces[x - 1, y]);
                            gridSpaces[x - 1, y].currBug = currentBug;
                            playingBugs[x - 1, y] = currentBug;
                            playingBugs[x, y] = null;
                            gridSpaces[x, y].currBug = null;
                        }
                        // If the next space is taken by an enemy, fight
                        else if (x - 1 >= 0 && playingBugs[x - 1, y] != null && playingBugs[x - 1, y].IsFriendly != currentBug.IsFriendly)
                        {
                            currentBug.Attack(playingBugs[x - 1, y]);
                            CheckDead(x, y);
                        }
                    }
                }
            }
            yield return new WaitForSeconds(1f);
        }
        gamePhase = GamePhase.PickingBugs;
    }

}
