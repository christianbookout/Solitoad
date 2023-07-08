using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caterpillar : Bug
{

    public override void SpecialAbility(int i, int j, GridSpace[,] gridSpaces)
    {
        // TODO set next bug to be webbed
    }

    public override void DealDamage(Bug bug, int damage = -1)
    {
        // TODO should isWebbed be set before either of them attack? Unsure
        bug.isWebbed = true;
        base.DealDamage(bug, damage);
    }
}
