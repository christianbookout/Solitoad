using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slug : Bug
{
    public override void TakeDamage(Bug fromBug, int damage = -1)
    {
        if (fromBug) fromBug.isPoisoned = true;
        base.TakeDamage(fromBug, damage);
    }
}
