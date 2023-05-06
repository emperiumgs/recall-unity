using UnityEngine;
using System.Collections;

public class Bullet : Projectile
{
	[HideInInspector]
	public int damage;

	void OnCollisionEnter2D(Collision2D hit)
	{
		IDamageable dmg = hit.collider.GetComponent<IDamageable>();
        IBlockable block = hit.collider.GetComponentInParent<IBlockable>();
        if (dmg != null)
            dmg.TakeDamage(damage);
        else if (block != null)
            block.Block();        
		Restore();
	}
}