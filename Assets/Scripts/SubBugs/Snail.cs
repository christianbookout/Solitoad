using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snail : Bug
{
    private bool HasBeenHit = false;

    public override void TakeDamage(Bug fromBug, int damage = -1)
    {
        if (HasBeenHit)
        {
            base.TakeDamage(fromBug, damage);
        }
        HasBeenHit = true;
    }
}
