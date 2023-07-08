using System.Collections.Generic;
using UnityEngine;

public class GridSpace : MonoBehaviour
{
    public Bug currBug;
    public bool friendlySquare = true;
    public bool canAddBugs = true;
    public int i;
    public int j;

    public bool PlayerCanAddBugs()
    {
        return currBug == null && canAddBugs && friendlySquare && GridController.gamePhase == GridController.GamePhase.PickingBugs;
    }

    public void AddBug(Bug bug)
    {
        bug.transform.position = transform.position;
        currBug = bug;
        canAddBugs = false;
    }

    public void RemoveBug()
    {
        currBug = null;
        if (friendlySquare) canAddBugs = true;
    }
}