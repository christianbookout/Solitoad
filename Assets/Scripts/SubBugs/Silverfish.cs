using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Silverfish : Bug
{
    [SerializeField] private float DodgeChance = 0.5f;

    public override void TakeDamage(Bug fromBug, int damage = -1)
    {
        if (Random.Range(0, 100) > DodgeChance * 100)
        {
            base.TakeDamage(fromBug, damage);
        }
    }
}
