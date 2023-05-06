using UnityEngine;
using System.Collections;

public class EnemyChaser : EnemyMelee
{
	public float speed;

	protected Rigidbody2D rb;

	Vector2 moveDir;

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
			currentState = Chase;
			anim.SetBool("chase", true);
		}
	}

	protected override void Engage()
	{
		FaceTarget();
		if (attackable)
			Attack();
		if (!atkRangeCollider)
		{
			currentState = Chase;
			anim.SetBool("chase", true);
		}
	}

	protected virtual void Chase()
	{
		FaceTarget();
		MoveTarget();
		if (atkRangeCollider)
		{
			anim.SetBool("chase", false);
			currentState = Engage;
		}
		else if (!rangeCollider)
		{
			anim.SetBool("chase", false);
			currentState = Idle;
			target = null;
		}
	}

	protected virtual void MoveTarget()
	{
		moveDir = rb.velocity;
		moveDir.x = transform.position.x > target.position.x ? -speed : speed;
		rb.velocity = moveDir;
	}

	protected override void Die()
	{
		base.Die();
		rb.isKinematic = true;
	}
}