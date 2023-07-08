using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Bug : MonoBehaviour
{
    public int Health;
    private int MaxHealth;
    public int AttackDamage;
    public string BugName;
    public float Speed;
    [SerializeField] 
    private float BreakWebChance = 0.5f;
    public bool moving = true;
    public bool IsFriendly = true;
    public bool isFighting = false;
    public bool isWebbed = false;
    public bool isPoisoned = false;
    [SerializeField]
    private int PoisonDamage = 1;
    private Image HealthRed;
    private Image HealthGreen;

    // Called always.
    public virtual void SpecialAbility(int i, int j, GridSpace[,] gridSpaces) { }

    // Called alongside an attack.
    public virtual void SpecialAttack(int i, int j, GridSpace[,] gridSpaces) { }

    public virtual void Die()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        Destroy(HealthBar);
    }

    public bool CanAttack(Bug other)
    {
        return other.IsFriendly != IsFriendly;
    }


    public virtual void TakeDamage(Bug fromBug = null, int damage = -1)
    {
        if (damage == -1)
        {
            // Ideally should never happen
            if (fromBug == null)
            {
                Debug.Log("THIS SHOULDNT HAPPEN");
                return;
            }
            damage = fromBug.AttackDamage;
        }
        Health -= damage;
    }

    public virtual void DealDamage(Bug toBug, int damage = -1)
    {
        // If you're webbed and the random number is greater than the probability of webbing then don't deal damage
        if (isWebbed && Random.Range(0, 100) > BreakWebChance * 100) return;
        if (damage == -1)
        {
            damage = AttackDamage;
        }
        toBug.TakeDamage(this, damage);
    }
    
    public bool SameTeam(Bug bug)
    {
        return bug != null && IsFriendly == bug.IsFriendly;
    }

    public IEnumerator Move(GridSpace to)
    {
        if (isPoisoned) TakeDamage(damage: PoisonDamage);

        return MoveAnimation(to);
    }

    protected virtual IEnumerator MoveAnimation(GridSpace to)
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

    public IEnumerator Attack(Bug target)
    {
        if (isPoisoned) TakeDamage(damage: PoisonDamage);
        if (target.Health > 0) DealDamage(target);
        return AttackAnimation(target);
    }

    protected virtual IEnumerator AttackAnimation(Bug target)
    {
        // Get the initial position of the bug
        float lungeTime = 0.2f; // the time the lunge should take in seconds
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

    }

    private Vector3 targetPosition;
    private Coroutine DoMoveAnimation;
    public GameObject HealthBar;
    private Canvas canvas;

    private void Awake()
    {
        PickNewTargetPosition();
        StartCoroutine(MoveToTargetPosition());
        canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
        HealthBar = Instantiate(HealthBar, canvas.transform);
        foreach (var comp in HealthBar.GetComponentsInChildren<Image>())
        {
            if (comp.gameObject.CompareTag("green"))
            {
                HealthGreen = comp;
                HealthGreen.enabled = false;
            } else if (comp.gameObject.CompareTag("red"))
            {
                HealthRed = comp;
                HealthRed.enabled = false;
            }
        }
        MaxHealth = Health;
    }

    private void PickNewTargetPosition()
    {
        // Here you need to specify the range where your bug can move.
        float randomX = Random.Range(-6f, -3.3f); // replace these with your scene boundaries
        float randomY = Random.Range(-1.25f, 3.3f); // replace these with your scene boundaries

        targetPosition = new Vector3(randomX, randomY, 0);
    }

    public void Update()
    {
        if (Health > MaxHealth) MaxHealth = Health;

        if ((isFighting || !moving) && DoMoveAnimation != null)
        {
            StopCoroutine(DoMoveAnimation);
            DoMoveAnimation = null;
        }
        else if (!isFighting && moving && DoMoveAnimation == null)
        {
            PickNewTargetPosition();
            DoMoveAnimation = StartCoroutine(MoveToTargetPosition());
        }
        if (Health == MaxHealth && HealthGreen && HealthRed)
        {
            HealthGreen.enabled = false;
            HealthRed.enabled = false;
        } else if (HealthGreen && HealthRed)
        {
            HealthBar.transform.position = transform.position;
            HealthGreen.enabled = true;
            HealthRed.enabled = true;
            float healthRatio = (float) Health / MaxHealth;
            Debug.Log("Health ratio is " + healthRatio);
            HealthGreen.fillAmount = healthRatio;
        }


        
    }


    private IEnumerator MoveToTargetPosition()
    {
        while (moving && Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;

            Vector3 movement = new(direction.x, direction.y, 0);

            GetComponent<SpriteRenderer>().flipX = movement.x < 0;

            transform.position += Speed * Time.deltaTime * movement;
            yield return null;
        }
        if (moving)
        {
            PickNewTargetPosition();
            DoMoveAnimation = StartCoroutine(MoveToTargetPosition());
        }
    }
}
