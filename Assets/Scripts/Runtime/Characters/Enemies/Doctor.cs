using UnityEngine;
using System.Collections;

public class Doctor : EliteSoldier
{
    public GameObject x;
    public Transform grenadeOrigin;
    public Grenade grenade;
    [Range(.1f, .99f)]
    public float grenadeChance = .3f;

    Vector2 leftDir = new Vector2(-10, .5f),
        rightDir = new Vector2(10, .5f);

    protected override void Awake()
    {
        base.Awake();
        grenade.damage = damage;
    }

    protected override void FaceTarget()
    {
        if (sr.flipX && target.position.x > transform.position.x)
        {
            foreach (SpriteRenderer s in sprites)
                s.flipX = false;
            origin.localPosition = new Vector3(-origin.localPosition.x, origin.localPosition.y);
            grenadeOrigin.localPosition = new Vector3(-grenadeOrigin.localPosition.x, grenadeOrigin.localPosition.y);
        }
        else if (!sr.flipX && target.position.x < transform.position.x)
        {
            foreach (SpriteRenderer s in sprites)
                s.flipX = true;
            origin.localPosition = new Vector3(-origin.localPosition.x, origin.localPosition.y);
            grenadeOrigin.localPosition = new Vector3(-grenadeOrigin.localPosition.x, grenadeOrigin.localPosition.y);
        }
    }

    protected override void Engage()
    {
        FaceTarget();

        if (attackable)
        {
            if (Random.Range(0, 1f) < grenadeChance)
                StartSpecial();
            else
                routine = StartCoroutine(CombatRoutine());
        }

        if (!rangeCollider)
        {
            if (!attackable && routine != null)
                AttackEnd();
            target = null;
            currentState = Idle;
            ResetBody();
        }
    }

    protected override void Die()
    {
        AttackEnd();
        ResetBody();
        health = 0;
        anim.SetTrigger("death");
        currentState = null;
    }

    void Turn()
    {
        gameObject.SetActive(false);
        x.SetActive(true);
    }

    void StartSpecial()
    {
        attackable = false;
        anim.SetTrigger("special");
        currentState = null;
    }

    void EndSpecial()
    {
        currentState = Engage;
        StartCoroutine(AttackCooldown());
    }

    void ThrowGrenade()
    {
        grenade.Fire(target.position.x > transform.position.x ? rightDir : leftDir);
    }
}