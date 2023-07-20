using UnityEngine;

public class Bullet : Projectile
{
    int _shieldLayer;

    [HideInInspector]
    public int damage;

    void Awake()
    {
        _shieldLayer = LayerMask.NameToLayer("Shield");
    }

    void OnCollisionEnter2D(Collision2D hit)
    {
        Restore();

        if (hit.collider.gameObject.layer == _shieldLayer)
        {
            var blockable = hit.collider.GetComponentInParent<IBlockable>();
            if (blockable != null)
            {
                blockable.Block();
                return;
            }
        }

        if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
            damageable.TakeDamage(damage);
    }
}