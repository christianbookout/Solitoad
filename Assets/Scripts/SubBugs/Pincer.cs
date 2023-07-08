using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pincer : Bug
{
    [SerializeField] private float CritChance = 0.5f;
    public override void DealDamage(Bug bug, int damage = -1)
    {
        if (Random.Range(0, 100) > CritChance * 100)
        {
            base.DealDamage(bug, damage);
        } else
        {
            if (bug) bug.GetComponent<SpriteRenderer>().flipY = true;
            base.DealDamage(bug, AttackDamage * 2);
        }
    }
}
