using UnityEngine;
using System.Collections;

public class Liss : MonoBehaviour, IDamageable, IBlockable
{
	enum JumpingStates
	{
		None,
		Start,
		Impulse,
		Air
	}

	public delegate void ToggleFocusEvent(bool value);
	public static event ToggleFocusEvent ToggleFocusHandler;

	public static Liss instance { get; private set; }

	[Range(1, 10)]
	public float speed,
		jumpForce;
    [Range(.5f, 3)]
    public float oozeDuration;
	public int[] comboDamage;
	public Vector2 atkPos,
		atkSize;
	public LayerMask colLayers;
    [HideInInspector]
    public Transform checkpoint;
    public AudioClip atkA,
        atkB,
        atkHit,
        takeDamage,
        block;

	// Got using Debug.DrawLine(pos, pos + Vector2.up * height);
	const float HEIGHT = 1.45f;
	const int MAX_HEALTH = 100;

	JumpingStates js;
	SpriteRenderer sr;
	SpriteRenderer knifeSr;
    BoxCollider2D col,
        shieldCol;
	HudController hud;
    AudioSource source;
	Rigidbody2D rb;
	Collider2D hit;
	GameObject shield,
		focusParticles;
    Coroutine ooze;
	Animator anim;
	Vector2 move,
		areaCheck;
	float h;
    bool active,
        facingRight,
        isGrounded,
        attackable,
        shielding,
        focus,
        takingDamage;
    int atkNum,
        health,
        enemyMask;

	Vector2 pos
	{
		get { return transform.position; }
	}
	Vector2 center
	{
		get { return pos + Vector2.up * HEIGHT / 2; }
	}
	Vector2 left
	{
		get { return center + Vector2.left * col.size.x / 2; }
	}
	Vector2 right
	{
		get { return center + Vector2.right * col.size.x / 2; }
	}

	void Awake()
	{
		instance = this;
		sr = GetComponent<SpriteRenderer>();
		rb = GetComponent<Rigidbody2D>();
		col = GetComponent<BoxCollider2D>();
		anim = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
		knifeSr = transform.GetChild(0).GetComponent<SpriteRenderer>();
		hud = HudController.instance;
		shield = transform.GetChild(1).gameObject;
        shieldCol = shield.GetComponent<BoxCollider2D>();
		focusParticles = transform.GetChild(2).gameObject;
		js = JumpingStates.None;
		health = MAX_HEALTH;
        enemyMask = 1 << LayerMask.NameToLayer("Enemy");
		areaCheck = new Vector2(.1f, HEIGHT - .1f);
		facingRight = true;
        attackable = true;
        active = true;
	}

	// Receive Inputs
	void Update()
	{
		if (!active)
			return;

		h = Input.GetAxis("Horizontal");

		if (js == JumpingStates.None && isGrounded && Input.GetButtonDown("Jump"))
			js = JumpingStates.Start;

		if (atkNum < comboDamage.Length + 1 && attackable && !shielding && !focus && Input.GetButtonDown("Fire1"))
			Attack();

        if (Input.GetKeyDown(KeyCode.P))
            hud.TogglePause();

		if (Input.GetButtonDown("Shield") && !focus)
			EnableShield();
		else if (Input.GetButtonUp("Shield"))
			DisableShield();

		if (Input.GetButtonDown("Focus") && isGrounded && !shielding && atkNum == 0)
			ToggleFocus();
	}

	void FixedUpdate()
	{
		if (atkNum > 0 || shielding || focus)
			return;

		FaceDirection();
		ControlMovement();		
	}

	void DealDamage()
	{
		if (atkNum == 0)
			return;
		Collider2D[] hits = Physics2D.OverlapBoxAll(center + atkPos, atkSize, 0, enemyMask);
		if (hits != null)
		{
            IDamageable dmg;
            foreach (Collider2D hit in hits)
            {
                dmg = hit.GetComponent<IDamageable>();
                if (dmg != null)
                {
                    dmg.TakeDamage(comboDamage[atkNum - 1]);
                    source.PlayOneShot(atkHit);
                }
            }
		}
	}

	void Attack()
	{
		atkNum++;
		anim.SetInteger("atkNum", atkNum);
        source.PlayOneShot(Random.Range(0, 2) > 0 ? atkA : atkB);
		DisableAttack();
		rb.isKinematic = true;
        rb.velocity = Vector2.zero;
		knifeSr.enabled = false;
	}

	void EnableAttack()
	{
		attackable = true;
	}

	void DisableAttack()
	{
		attackable = false;
	}

	void ClearAttack()
	{
		atkNum = 0;
		anim.SetInteger("atkNum", 0);
		EnableAttack();
		rb.isKinematic = false;
		if (!shielding)
			knifeSr.enabled = true;
	}

	void EnableShield()
	{
		shield.gameObject.SetActive(true);
		shielding = true;
		anim.SetBool("shield", true);
		knifeSr.enabled = false;
		ActionTrigger();
		if (atkNum > 0)
			ClearAttack();
	}

	void DisableShield()
	{
		shield.gameObject.SetActive(false);
		shielding = false;
		knifeSr.enabled = true;
		anim.SetBool("shield", false);
	}

	void ControlMovement()
	{
		rb.velocity = new Vector2(h * speed, rb.velocity.y);
		if (js == JumpingStates.Start)
		{
			js = JumpingStates.Impulse;
			rb.velocity = new Vector2(rb.velocity.x, jumpForce);
			anim.SetBool("jump", true);
			isGrounded = false;
		}
		else if (js == JumpingStates.Air)
		{
			if (Physics2D.OverlapBox(left, areaCheck, 0, colLayers) || Physics2D.OverlapBox(right, areaCheck, 0, colLayers))
			{
				rb.velocity = new Vector2(0, rb.velocity.y);
			}
		}
		if (js != JumpingStates.Impulse && Physics2D.OverlapCircle(pos, .001f, colLayers))
		{
			isGrounded = true;
			if (js == JumpingStates.Air)
			{
				js = JumpingStates.None;
				anim.SetBool("jump", false);
			}
		}
		if (js == JumpingStates.Impulse)
			js = JumpingStates.Air;

		anim.SetBool("walking", h != 0 && js == JumpingStates.None);
	}

	void FaceDirection()
	{
		if (facingRight && h < 0)
		{
			facingRight = false;
			sr.flipX = true;
			knifeSr.flipX = true;
			Vector3 pos = focusParticles.transform.localPosition;
			pos.x *= -1;
			focusParticles.transform.localPosition = pos;
            atkPos.x *= -1;
            shieldCol.offset = new Vector2(-shieldCol.offset.x, shieldCol.offset.y);
		}
		else if (!facingRight && h > 0)
		{
			facingRight = true;
			sr.flipX = false;
			knifeSr.flipX = false;
			Vector3 pos = focusParticles.transform.localPosition;
			pos.x *= -1;
			focusParticles.transform.localPosition = pos;
            atkPos.x *= -1;
            shieldCol.offset = new Vector2(-shieldCol.offset.x, shieldCol.offset.y);
        }
	}

	void ActionTrigger()
	{
		anim.SetTrigger("trigger");
		rb.velocity = new Vector2(0, rb.velocity.y);
	}
    
    void Respawn()
    {
        transform.position = checkpoint.position;
        anim.SetBool("death", false);
        SetInteractive(true);
    }

	public void ToggleFocus()
	{
		focus = !focus;
		rb.isKinematic = focus;
		focusParticles.SetActive(focus);
		if (ToggleFocusHandler != null)
			ToggleFocusHandler(focus);
		anim.SetBool("focus", focus);
		if (focus)
			ActionTrigger();
	}

	public void TakeDamage(int damage)
	{
        if (!active)
            return;
		health -= damage;
        source.PlayOneShot(takeDamage);
        hud.UpdateHealth((float)health / MAX_HEALTH);
        if (health <= 0)
		{
            health = MAX_HEALTH;
            hud.UpdateHealth(1);
            SetInteractive(false);
            anim.SetBool("death", true);
            anim.SetTrigger("trigger");
            StartCoroutine(RespawnRoutine());
            return;
        }
        if (ooze == null)
        {
            if (!takingDamage)
            {
                SetInteractive(false);
                takingDamage = true;
            }
            anim.SetTrigger("damage");
        }
	}

    public void EndDamage()
    {
        takingDamage = false;
        SetInteractive(true);
    }

    public void Block()
    {
        source.PlayOneShot(block);
    }

	public void Ooze()
	{
        if (ooze == null)
        {
            SetInteractive(false);
            anim.SetBool("ooze", true);
            ooze = StartCoroutine(OozeDuration());
        }
        else
        {
            StopCoroutine(ooze);
            ooze = StartCoroutine(OozeDuration());
        }
	}

	public void SetInteractive(bool value)
	{
		active = value;
		if (!value)
		{
			h = 0;
			if (focus)
				ToggleFocus();
			if (shield)
				DisableShield();
			if (atkNum > 0)
				ClearAttack();
		}
	}

	IEnumerator OozeDuration()
	{
		yield return new WaitForSeconds(oozeDuration);
		SetInteractive(true);
		anim.SetBool("ooze", false);
        ooze = null;
	}

    IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(2);
        Respawn();
    }
}