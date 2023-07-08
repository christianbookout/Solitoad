using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worm : Bug
{
    [SerializeField]
    private int HealthBuff = 1;

    public override void SpecialAbility(int i, int j, GridSpace[,] gridSpaces) 
    {
        var dir = IsFriendly ? 1 : -1;
        if (i + dir < gridSpaces.GetLength(0) && i + dir >= 0 && gridSpaces[i + dir, j].currBug && gridSpaces[i + dir, j].currBug.IsFriendly == IsFriendly)
        {
            Debug.Log("Adding health to " + gridSpaces[i + dir, j].currBug.BugName);
            gridSpaces[i + dir, j].currBug.Health += HealthBuff;
        }
    }
}
