using UnityEngine;
using System.Collections;

public class Morbius : EnemyMelee
{
	public Transform origin;
	public Ooze ooze;
	[Range(.01f, .99f)]
	public float oozeChance;
    public AudioClip spit;

    Rigidbody2D rb;
	Coroutine oozeAttack;
    Vector2 leftDir = new Vector2(.5f, -.5f),
        rightDir = new Vector2(-.5f, -.5f);

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
    }

    protected override void Idle()
	{
		if (rangeCollider)
		{
			target = rangeCollider.transform;
			currentState = Engage;
		}
	}

	protected override void Engage()
	{
		FaceTarget();
		if (attackable)
		{
			if (oozeAttack == null)
				oozeAttack = StartCoroutine(OozeAttack());
			if (atkRangeCollider)
				Attack();
		}
		if (!rangeCollider)
		{
			target = null;
			currentState = Idle;
		}
	}

	protected override void FaceTarget()
	{
		if (sr.flipX && target.position.x > transform.position.x)
		{
			sr.flipX = false;
			origin.localPosition = new Vector3(-origin.localPosition.x, origin.localPosition.y);
			meleeRangePos = new Vector2(-meleeRangePos.x, meleeRangePos.y);
		}
		else if (!sr.flipX && target.position.x < transform.position.x)
		{
			sr.flipX = true;
			origin.localPosition = new Vector3(-origin.localPosition.x, origin.localPosition.y);
			meleeRangePos = new Vector2(-meleeRangePos.x, meleeRangePos.y);
		}
	}

    protected override void Die()
    {
        base.Die();
        rb.isKinematic = false;
    }

    void SpitOoze()
	{
		attackable = false;
		StartCoroutine(AttackCooldown());
		ooze.Fire(target.position.x > transform.position.x ? leftDir : rightDir);
	}

	IEnumerator OozeAttack()
	{
		while (attackable && currentState == Engage)
		{
            if (Random.Range(0, 1f) <= oozeChance)
            {
                SpitOoze();
                source.PlayOneShot(spit);
            }
			yield return new WaitForSeconds(1);
		}
		oozeAttack = null;
	}
}