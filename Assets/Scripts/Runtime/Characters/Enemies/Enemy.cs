using UnityEngine;
using System.Collections;

public abstract class Enemy : MonoBehaviour, IDamageable
{
	public const float DEATH_TIME = 3f;

	public float range;
	[Range(.1f, 3f)]
	public float minAtkCooldown,
		maxAtkCooldown;
	public int maxHealth,
		damage;

	protected delegate void State();

	protected State currentState;

	protected SpriteRenderer sr;
	protected BoxCollider2D col;
    protected AudioSource source;
	protected Transform target;
	protected Animator anim;
	protected bool attackable;
	protected int health,
		mask;

	protected Collider2D rangeCollider
	{
		get { return Physics2D.OverlapCircle(transform.position, range, mask); }
	}
    
    int deathLayer;

    protected virtual void Awake()
	{
		anim = GetComponent<Animator>();
		sr = GetComponent<SpriteRenderer>();
		col = GetComponent<BoxCollider2D>();
        source = GetComponent<AudioSource>();
		mask = 1 << LayerMask.NameToLayer("Player");
        deathLayer = LayerMask.NameToLayer("InactiveEnemy");
        currentState = Idle;
		health = maxHealth;
		attackable = true;
	}

	protected void FixedUpdate()
	{
		if (currentState != null)
			currentState();
	}

	protected virtual void Idle()
	{
		if (rangeCollider)
		{
			target = rangeCollider.transform;
			currentState = Engage;
		}
	}

	protected virtual void Engage()
	{
		FaceTarget();
        
		if (!rangeCollider)
		{
			target = null;
			currentState = Idle;
		}
	}

	protected virtual void FaceTarget()
	{
		if (sr.flipX && target.position.x > transform.position.x)
			sr.flipX = false;
		else if (!sr.flipX && target.position.x < transform.position.x)
			sr.flipX = true;
	}

	protected virtual void Die()
	{
		health = 0;
		anim.SetTrigger("death");
        gameObject.layer = deathLayer;
        StartCoroutine(Death());
		currentState = null;
	}

	public virtual void TakeDamage(int damage)
	{
		health -= damage;
		if (health <= 0)
			Die();
	}

	protected virtual void AttackEnd()
	{
		currentState = Engage;
		StartCoroutine(AttackCooldown());
	}

	protected virtual IEnumerator Death()
	{
        yield return new WaitForSeconds(DEATH_TIME);
		gameObject.SetActive(false);
	}

	protected virtual IEnumerator AttackCooldown()
	{
		yield return new WaitForSeconds(Random.Range(minAtkCooldown, maxAtkCooldown));
		attackable = true;
	}

	protected virtual void OnDrawGizmosSelected()
	{
		var color = Gizmos.color;
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, range);
		Gizmos.color = color;
	}
}