using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beetle : Bug
{
    [SerializeField] private int HealthRegen = 3;
    public Beetle()
    {
        Health = 10;
        AttackDamage = 10;
        BugName = "Beetle";
        Speed = 1f;
    }

    public override void DealDamage(Bug other)
    {
        Health += HealthRegen;
        base.DealDamage(other);
    }
}
