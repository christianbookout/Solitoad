using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

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
    private SpriteRenderer speechBubble;
    private SpriteRenderer attack;
    private SpriteRenderer march;

    public float spacing = 0.3f;

    private void Start()
    {
        gridSpaces = new GridSpace[width, height];
        playingBugs = new Bug[width, height];
        CreateGrid();
    }

    private void Awake()
    {
        speechBubble = GameObject.FindGameObjectWithTag("SpeechBubble").GetComponent<SpriteRenderer>();
        attack = GameObject.FindGameObjectWithTag("Attack").GetComponent<SpriteRenderer>();
        march = GameObject.FindGameObjectWithTag("Move").GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        
        if (gamePhase == GamePhase.PickingBugs)
        {
            if (!Input.GetKeyDown(KeyCode.Space)) 
            {
                for (int i = 0; i < width/2; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        if (gridSpaces[i, j].currBug == null) return;
                    }
                }
            }
            gamePhase = GamePhase.Battling;
            AddEnemies();
            RegisterBugs();

            StartCoroutine(StepOne());
        }
    }

    private void CreateGrid()
    {
        foreach (var gridSpace in GameObject.FindGameObjectsWithTag("GridSquare"))
        {
            if (gridSpace.TryGetComponent<GridSpace>(out var space))
            {
                if (space.i >= width / 2)
                {
                    space.canAddBugs = false;
                }
                gridSpaces[space.i, space.j] = space;
            }
        }
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
                bug.IsFriendly = false;
                gridSpaces[i, j].currBug = bug;
                gridSpaces[i, j].friendlySquare = false;
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

    private bool IsFinished()
    {
        for (int j = 0; j < height; j++)
        {
            var allFriendly = true;
            var allEnemies = true;
            var rowEmpty = true;
            for (int i = 0; i < width; i++)
            {
                if (playingBugs[i, j] != null)
                {
                    rowEmpty = false;
                    if (playingBugs[i, j].IsFriendly)
                    {
                        allEnemies = false;
                    } else if (!playingBugs[i, j].IsFriendly)
                    {
                        allFriendly = false;
                    }
                }
            }
            var anyOpposingBugs = !allEnemies && !allFriendly;
            // If the row has buggies and they are not all enemies or all friendly then we are still fighting.
            if (!rowEmpty && anyOpposingBugs) return false;
        }
        return true;
    }


    /* Bug turn logic:
     * For each column from 1 to width-2: 
     *     For each row from 0 to height - 1 
     *         If the piece exists and can attack or move, add the attack coroutine or move coroutine to the coroutine list like Coroutines.Add(ActivateCoroutine(bug.Attack(otherBug)))
     *         Always activate special abilities!!!
     *     Wait until all coroutines in the list are done (null).
     * Check if the game is over. 
     *     If the player has more bugs than the opponent, the player wins. Same works the other way. Same amount means tie. Clear all bugs from the board.
     *
     * 
     */

    private YieldCollection manager = new();

    private void ResetBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (playingBugs[i, j])
                {
                    playingBugs[i, j].IsFriendly = true;
                    playingBugs[i, j].moving = true;
                    playingBugs[i, j].isFighting = false;
                    playingBugs[i, j] = null;
                }
                gridSpaces[i, j].currBug = null;
                gridSpaces[i, j].canAddBugs = i < width / 2;
                gridSpaces[i, j].friendlySquare = i < width / 2;
            }
        }
    }

    private void CheckAllDead()
    {

        for (var i = 0; i < width; i++)
        {
            for (var j = 0; j < height; j++)
            {
                if (playingBugs[i, j] && playingBugs[i, j].Health <= 0)
                {
                    playingBugs[i, j].Die();
                    playingBugs[i, j] = null;
                    gridSpaces[i, j].currBug = null;
                }
            }
        }
    }

    // Battle phase
    private IEnumerator StepOne()
    {
        while (!IsFinished())
        {
            for (var i = 0; i < width; i++)
            {
                yield return new WaitForSeconds(0.1f);
                for (var j = height-1;  j >= 0; j--)
                {
                    if (playingBugs[i, j] != null)
                    {
                        Bug bug = playingBugs[i, j];
                        bug.SpecialAbility(i, j, gridSpaces);
                        var dir = bug.IsFriendly ? 1 : -1;
                        bool safeMove = i + dir < width && i + dir >= 0;
                        if (safeMove && playingBugs[i + dir, j] && bug.CanAttack(playingBugs[i + dir, j]))
                        {
                            speechBubble.enabled = true;
                            attack.enabled = true;
                            march.enabled = false;
                            // TODO only do this for friendly bugs. This should make the opponent bug attack this WHILE this attacks the opponent - therefore, handling damage correct.
                            var attackCoroutine = bug.Attack(playingBugs[i + dir, j]);
                            bug.SpecialAttack(i, j, gridSpaces);
                            StartCoroutine(manager.CountCoroutine(attackCoroutine));
                        } 
                    }
                }
            }
            yield return manager;
            CheckAllDead();

            for (var i = width - 1; i >= 0; i--)
            {
                yield return new WaitForSeconds(0.1f);
                for (var j = height - 1; j >= 0; j--)
                {
                    CheckMove(i, j, true);
                    CheckMove(width - i - 1, j, false);
                }
            }
            yield return manager;

            yield return new WaitForSeconds(0.5f);
        }
        speechBubble.enabled = false;
        attack.enabled = false;
        march.enabled = false;
        ResetBoard();
        gamePhase = GamePhase.PickingBugs;
    }

    private void CheckMove(int i, int j, bool IsFriendly)
    {
        if (playingBugs[i, j] != null && playingBugs[i, j].IsFriendly == IsFriendly)
        {
            Bug bug = playingBugs[i, j];
            var indx = i;
            var dir = bug.IsFriendly ? 1 : -1;
            bool safeMove = indx + dir < width && indx + dir >= 0;
            if (safeMove && playingBugs[indx + dir, j] == null)
            {
                speechBubble.enabled = true;
                attack.enabled = false;
                march.enabled = true;
                var move = bug.Move(gridSpaces[indx + dir, j]);
                gridSpaces[indx + dir, j].currBug = playingBugs[indx, j];
                playingBugs[indx + dir, j] = playingBugs[indx, j];
                playingBugs[indx, j] = null;
                gridSpaces[indx, j].currBug = null;
                StartCoroutine(manager.CountCoroutine(move));
            }
        }
    }
}
