using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beetle : Bug
{
    [SerializeField] private int HealthRegen = 3;

    public override void DealDamage(Bug other, int damage)
    {
        Health += HealthRegen;
        base.DealDamage(other, damage);
    }
}
