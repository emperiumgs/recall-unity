using UnityEngine;
using System.Collections;

public class EliteSoldier : Enemy
{
	public SpriteRenderer[] sprites;
	public GameObject legs;
	public Transform aim,
		origin;
	public Bullet bullet;
	[Range(0.2f, 5f)]
	public float minAimDuration = 1.5f,
		maxAimDuration = 2.5f,
		minShootDuration = .25f,
		maxShootDuration = .5f,
		minWaitDuration = .5f,
		maxWaitDuration = 1.5f;
	[Range(-45, 45)]
	public int minRotateAngle = -30,
		maxRotateAngle = 30;
    public AudioClip shoot;

	protected Coroutine routine;

	protected override void Awake()
	{
		base.Awake();
		bullet.damage = damage;
	}

	protected override void Engage()
	{
		FaceTarget();

		if (attackable)
			routine = StartCoroutine(CombatRoutine());

		if (!rangeCollider)
		{
			if (!attackable && routine != null)
				AttackEnd();
			target = null;
			currentState = Idle;
			ResetBody();
		}
	}

	protected override void FaceTarget()
	{
		if (sr.flipX && target.position.x > transform.position.x)
		{
			foreach (SpriteRenderer s in sprites)
				s.flipX = false;
			origin.localPosition = new Vector3(-origin.localPosition.x, origin.localPosition.y);
		}
		else if (!sr.flipX && target.position.x < transform.position.x)
		{
			foreach (SpriteRenderer s in sprites)
				s.flipX = true;
			origin.localPosition = new Vector3(-origin.localPosition.x, origin.localPosition.y);
		}
	}

	protected override void Die()
	{
		AttackEnd();
		ResetBody();
		base.Die();
	}

	protected override void AttackEnd()
	{
		if (routine != null)
		{
			StopCoroutine(routine);
			routine = null;
		}
		base.AttackEnd();
	}

	protected void ResetBody()
	{
		legs.SetActive(false);
		sr.enabled = true;
	}

	protected IEnumerator CombatRoutine()
	{
		attackable = false;
		legs.SetActive(true);
		sr.enabled = false;
		float time = 0,
		angle;
		int mod;
		Vector2 lookDir,
		targetDir = Vector2.zero;
		float maxTime = Random.Range(minAimDuration, maxAimDuration);
		while (time < maxTime)
		{
			time += Time.fixedDeltaTime;
			lookDir = sr.flipX ? Vector2.left : Vector2.right;
			mod = sr.flipX ? -1 : 1;
			targetDir = target.position - transform.position;
			angle = Vector2.Angle(lookDir, targetDir);
			angle = Mathf.Clamp(angle, minRotateAngle, maxRotateAngle);
			aim.localEulerAngles = new Vector3(0, 0, mod * angle);
			yield return new WaitForFixedUpdate();
		}
		yield return new WaitForSeconds(Random.Range(minShootDuration, maxShootDuration));
		ResetBody();
		bullet.Fire(targetDir);
        source.PlayOneShot(shoot);
		yield return new WaitForSeconds(Random.Range(minWaitDuration, maxWaitDuration));
		routine = null;
		AttackEnd();
	}
}