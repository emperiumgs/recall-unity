using UnityEngine;
using System.Collections;

public class EnemyMelee : Enemy
{
    public AudioClip atkSwing,
        atkHit;
	public Vector2 meleeRangePos,
		meleeRangeSize;

	int shieldMask;

	protected Collider2D atkRangeCollider
	{
		get { return Physics2D.OverlapBox(transform.position + (Vector3)meleeRangePos, meleeRangeSize, 0, mask); }
	}

	protected override void Awake()
	{
		base.Awake();
		shieldMask = 1 << LayerMask.NameToLayer("Shield");
	}

	protected override void FaceTarget()
	{
		if (sr.flipX && target.position.x > transform.position.x)
		{
			sr.flipX = false;
			meleeRangePos = new Vector2(-meleeRangePos.x, meleeRangePos.y);
		}
		else if (!sr.flipX && target.position.x < transform.position.x)
		{
			sr.flipX = true;
			meleeRangePos = new Vector2(-meleeRangePos.x, meleeRangePos.y);
		}
	}

	protected virtual void Attack()
	{
		anim.SetTrigger("attack");
		currentState = null;
		attackable = false;
	}

	protected virtual void AttackDamage()
	{
        if (atkSwing)
            source.PlayOneShot(atkSwing);
        Collider2D hit = Physics2D.OverlapBox((Vector2)transform.position + meleeRangePos, meleeRangeSize, 0, shieldMask);
        if (hit != null)
        {
            hit.GetComponentInParent<IBlockable>().Block();
            return;
        }

		hit = Physics2D.OverlapBox((Vector2)transform.position + meleeRangePos, meleeRangeSize, 0, mask);
        if (hit)
        {
            hit.GetComponent<IDamageable>().TakeDamage(damage);
            if (atkHit)
                source.PlayOneShot(atkHit);
        }
	}

	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		Gizmos.DrawWireCube(transform.position + (Vector3)meleeRangePos, meleeRangeSize);
	}
}