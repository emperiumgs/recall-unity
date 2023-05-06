using UnityEngine;
using System.Collections;

public abstract class Projectile : MonoBehaviour
{
    public float speed,
        resetTime = 3f;

    protected CircleCollider2D col;
    protected SpriteRenderer sr;
    protected Rigidbody2D rb;
    protected Transform parent;
    protected Coroutine timer;
    protected Vector2 dir;
    protected bool active;

	protected void Setup()
	{
		parent = transform.parent;
		col = GetComponent<CircleCollider2D>();
		sr = GetComponent<SpriteRenderer>();
		rb = GetComponent<Rigidbody2D>();
	}

	protected virtual void FixedUpdate()
	{
		if (!active)
			return;

		rb.velocity = dir;
	}

	protected virtual void Restore()
	{
		Restore(false);
	}

	protected virtual void Restore(bool positionOnly)
	{
		if (timer != null)
		{
			StopCoroutine(timer);
			timer = null;
		}
		transform.SetParent(parent);
		transform.localPosition = Vector3.zero;
		transform.localEulerAngles = Vector3.zero;
		if (!positionOnly)
		{
			active = false;
			gameObject.SetActive(false);
		}
	}

	public virtual void Fire(Vector2 targetDir)
	{
		if (active)
			Restore(true);
		else
		{
			gameObject.SetActive(true);
			if (!parent)
				Setup();
			active = true;
		}
		dir = targetDir.normalized * speed;
		transform.SetParent(null);
		if (dir.x > 0 && sr.flipX)
		{
			sr.flipX = false;
			col.offset = new Vector2(-col.offset.x, col.offset.y);
		}
		else if (dir.x < 0 && !sr.flipX)
		{
			sr.flipX = true;
			col.offset = new Vector2(-col.offset.x, col.offset.y);
		}
		timer = StartCoroutine(ResetTimer());
	}

	protected IEnumerator ResetTimer()
	{
		yield return new WaitForSeconds(resetTime);
		timer = null;
		Restore();
	}
}