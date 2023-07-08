using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugManager : MonoBehaviour
{
    public float spawnInterval = 5f;  // Spawn a new bug every 5 seconds
    public Vector2 minBounds;  // The minimum x and y coordinates where bugs can spawn and move
    public Vector2 maxBounds;  // The maximum x and y coordinates where bugs can spawn and move
    public Bug[] bugs;
    private int bugCount;
    private Coroutine bugSpawner;
    public int maxBugs;

    void Update()
    {
        if (GridController.gamePhase == GridController.GamePhase.PickingBugs && bugSpawner == null)
            bugSpawner = StartCoroutine(SpawnBugs());
        else if (GridController.gamePhase != GridController.GamePhase.PickingBugs && bugSpawner != null)
        {
            StopCoroutine(bugSpawner);
            bugSpawner = null;
            bugCount = 0;
        }
    }


    private IEnumerator SpawnBugs()
    {
        while (bugCount < maxBugs)
        {
            var bugPosition = new Vector3(Random.Range(minBounds.x, maxBounds.x), Random.Range(minBounds.y, maxBounds.y), 0);
            var bugIndex = Random.Range(0, bugs.Length);
            Instantiate(bugs[bugIndex], bugPosition, Quaternion.identity);
            bugCount++;
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
