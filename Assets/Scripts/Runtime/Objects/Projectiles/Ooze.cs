using UnityEngine;
using Recall.Gameplay.Interfaces;
using MyGameDevTools.Extensions;

public class Ooze : Projectile
{
    [SerializeField]
    LayerMask _hitMask;

    bool bounced;

	void OnCollisionEnter2D(Collision2D hit)
	{
        if (_hitMask.HasLayer(hit.collider.gameObject.layer))
        {
            Restore();
            hit.collider.GetComponentInParent<IOozable>()?.ApplyOoze();
            return;
        }

        if (bounced)
            Restore();
        else
        {
            dir.y *= -1;
            sr.flipY = true;
            col.offset = new Vector2(col.offset.x, -col.offset.y);
            bounced = true;
        }
	}

    protected override void Restore(bool positionOnly)
    {
        base.Restore(positionOnly);
        sr.flipY = false;
        col.offset = new Vector2(col.offset.x, -col.offset.y);
        bounced = false;
    }
}