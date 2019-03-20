using UnityEngine;
using System.Collections;

public class ShockTrooper : EnemyChaser
{
    public Sprite brokenShield;
    public AudioClip breakShield;

    SpriteRenderer shield;
    Transform shieldT;

    protected override void Awake()
    {
        base.Awake();
        shieldT = transform.GetChild(0);
        shield = shieldT.GetComponent<SpriteRenderer>();
    }

    protected override void FaceTarget()
    {
        if (sr.flipX && target.position.x > transform.position.x)
        {
            sr.flipX = false;
            shield.flipX = false;
            shieldT.localPosition = new Vector3(-shieldT.localPosition.x, shieldT.localPosition.y);
            meleeRangePos = new Vector2(-meleeRangePos.x, meleeRangePos.y);
        }
        else if (!sr.flipX && target.position.x < transform.position.x)
        {
            sr.flipX = true;
            shield.flipX = true;
            shieldT.localPosition = new Vector3(-shieldT.localPosition.x, shieldT.localPosition.y);
            meleeRangePos = new Vector2(-meleeRangePos.x, meleeRangePos.y);
        }
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        if (shield.enabled)
        {
            if (shield.enabled && health < maxHealth / 2)
            {
                shield.enabled = false;
                source.PlayOneShot(breakShield);
            }
            else if (health < 3 * maxHealth / 4)
                shield.sprite = brokenShield;
        }
    }
}