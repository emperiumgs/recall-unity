using UnityEngine;
using System.Collections;

public class Grenade : Projectile
{
    [HideInInspector]
    public int damage;

    ParticleSystem explosion;
    Coroutine explode;
    int mask;

    void Awake()
    {
        explosion = GetComponentInChildren<ParticleSystem>();
        mask = 1 << LayerMask.NameToLayer("Player");
    }

    protected override void FixedUpdate()
    {        
    }

    protected override void Restore(bool positionOnly)
    {
        base.Restore(positionOnly);
        col.enabled = true;
        rb.isKinematic = false;
        sr.enabled = true;
        if (explode != null)
            StopCoroutine(explode);
    }

    public override void Fire(Vector2 targetDir)
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
        dir = targetDir * speed;
        transform.SetParent(null);
        rb.AddForce(dir, ForceMode2D.Impulse);
        explode = StartCoroutine(ExplosionTimer());
        timer = StartCoroutine(ResetTimer());
    }

    IEnumerator ExplosionTimer()
    {
        yield return new WaitForSeconds(2);
        sr.enabled = false;
        col.enabled = false;
        rb.isKinematic = true;
        explosion.Play();
        Collider2D hit = Physics2D.OverlapCircle(transform.position, 1, mask);
        if (hit != null)
            hit.GetComponent<IDamageable>().TakeDamage(damage);
        explode = null;
    }
}
