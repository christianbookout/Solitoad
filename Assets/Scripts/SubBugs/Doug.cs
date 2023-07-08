using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Doug : Bug
{
    public GameObject sandParticlesPrefab;
    public override void DealDamage(Bug bug, int damage){}

    protected override IEnumerator AttackAnimation(Bug target)
    {
        Debug.Log("here");
        // Instantiate the particle system at the desired position
        GameObject sandParticles = Instantiate(sandParticlesPrefab, transform.position, transform.rotation);

        // Start the particle system
        ParticleSystem ps = sandParticles.GetComponent<ParticleSystem>();
        if (GetComponent<SpriteRenderer>().flipX) sandParticles.transform.Rotate(0, 180, 0);
        ps.Play();
        yield return new WaitForSeconds(ps.main.duration);
    }

    public override void SpecialAttack(int i, int j, GridSpace[,] gridSpaces)
    {
        var dir = IsFriendly ? 1 : -1;
        if (i + dir < gridSpaces.GetLength(0) && i + dir >= 0)
        {
            if (gridSpaces[i + dir, j].currBug && CanAttack(gridSpaces[i + dir, j].currBug))
            {
                base.DealDamage(gridSpaces[i + dir, j].currBug);
            }
            if (i + dir + dir < gridSpaces.GetLength(0) && i + dir + dir >= 0 && gridSpaces[i + dir + dir, j].currBug && CanAttack(gridSpaces[i + dir + dir, j].currBug))
            {
                base.DealDamage(gridSpaces[i + dir + dir, j].currBug, AttackDamage / 2);
            }
        }
    }
}
