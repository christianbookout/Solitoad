using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;


public class Bug : MonoBehaviour
{
    public int Health;
    public int AttackDamage;
    public string BugName;
    public float Speed;
    public bool moving = true;
    public bool IsFriendly = true;
    public bool isFighting = false;


    public void TakeDamage(int damage)
    {
        Health -= damage;
    }

    public virtual void DealDamage(Bug bug)
    {
        bug.TakeDamage(AttackDamage);
    }
    
    public virtual bool SameTeam(Bug bug)
    {
        return bug != null && IsFriendly == bug.IsFriendly;
    }

    public Coroutine Move(GridSpace to)
    {
        return StartCoroutine(MoveCoroutine(to));
    }

    protected virtual IEnumerator MoveCoroutine(GridSpace to)
    {
        float moveTime = 0.3f; // time it takes to move
        float elapsed = 0; // time that has elapsed

        Vector3 originalPosition = transform.position;
        Vector3 targetPosition = to.transform.position;

        while (elapsed < moveTime)
        {
            float ratio = elapsed / moveTime;
            transform.position = Vector3.Lerp(originalPosition, targetPosition, ratio);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the bug is exactly at the target position
        transform.position = targetPosition;
    }

    public Coroutine Attack(Bug target)
    {
        return StartCoroutine(AttackCoroutine(target));
    }

    protected virtual IEnumerator AttackCoroutine(Bug target)
    {
        // Get the initial position of the bug
        float lungeTime = 0.3f; // the time the lunge should take in seconds
        float elapsed = 0; // the time that has already passed

        Vector3 originalPosition = transform.position;
        Vector3 targetPosition = (target.transform.position - originalPosition)/3 + originalPosition;

        while (elapsed < lungeTime)
        {
            float ratio = elapsed / lungeTime;
            transform.position = Vector3.Lerp(originalPosition, targetPosition, ratio);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure it's really at the target position
        transform.position = targetPosition;

        elapsed = 0; // reset the elapsed time
        while (elapsed < lungeTime)
        {
            float ratio = elapsed / lungeTime;
            transform.position = Vector3.Lerp(targetPosition, originalPosition, ratio);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure it's back at the original position
        transform.position = originalPosition;

        DealDamage(target);
    }

    private Vector3 targetPosition;
    private Coroutine moveCoroutine;


    private void Awake()
    {
        PickNewTargetPosition();
        StartCoroutine(MoveToTargetPosition());
    }

    private void PickNewTargetPosition()
    {
        // Here you need to specify the range where your bug can move.
        float randomX = Random.Range(-7f, -1f); // replace these with your scene boundaries
        float randomY = Random.Range(-3f, 3f); // replace these with your scene boundaries

        targetPosition = new Vector3(randomX, randomY, 0);
    }

    public void Update()
    {
        if (isFighting)
        {
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
                moveCoroutine = null;
            }
            return;
        }
        if (!moving && moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        } else if (moving && moveCoroutine == null)
        {
            PickNewTargetPosition();
            moveCoroutine = StartCoroutine(MoveToTargetPosition());
        }
    }

    private IEnumerator MoveToTargetPosition()
    {
        while (moving && Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;

            Vector3 movement = new Vector3(direction.x, direction.y, 0);

            GetComponent<SpriteRenderer>().flipX = movement.x < 0;

            transform.position += speed * Time.deltaTime * movement;
            yield return null;
        }
        if (moving)
        {
            PickNewTargetPosition();
            moveCoroutine = StartCoroutine(MoveToTargetPosition());
        }
    }

}
