using UnityEngine;
using System.Collections;

public class Ooze : Projectile
{
    const string TARGET_TAG = "Player";
    
    bool bounced;
    int shieldLayer;

    void Awake()
    {
        shieldLayer = LayerMask.NameToLayer("Shield");
    }

	void OnCollisionEnter2D(Collision2D hit)
	{
        if (hit.collider.tag == TARGET_TAG || hit.collider.gameObject.layer == shieldLayer)
        {
            hit.collider.GetComponentInParent<Liss>().Ooze();
            Restore();
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