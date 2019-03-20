using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class XDoctor : EnemyMelee
{
    enum AttackType
    {
        Slash,
        Claw,
        Ooze,
        Spike
    }

    public Bullet[] shards;
    public Ooze ooze;

    AttackType t;
    Vector2 shardDir = Vector2.left,
        oozeDir = new Vector2(-.5f, -.5f);
    string[] atks = { "slash", "claw", "ooze", "spike" };

    protected override void Awake()
    {
        base.Awake();
        foreach (Bullet shard in shards)
            shard.damage = damage;
    }

    protected override void Engage()
    {
        if (attackable)
        {
            float r = Random.Range(0, 1f);
            currentState = null;
            attackable = false;
            if (atkRangeCollider)
            {
                if (r < .25f)
                    t = AttackType.Slash;
                else if (r < .5f)
                    t = AttackType.Claw;
                else if (r < .75f)
                    t = AttackType.Ooze;
                else
                    t = AttackType.Spike;
            }
            else
                t = r < .5f ? AttackType.Ooze : AttackType.Spike;
            anim.SetTrigger(atks[(int)t]);
        }
    }

    void ShootShards()
    {
        foreach (Bullet shard in shards)
            shard.Fire(shardDir);
    }

    void SpitOoze()
    {
        ooze.Fire(oozeDir);
    }

    protected override IEnumerator Death()
    {
        yield return new WaitForSeconds(DEATH_TIME);
        SceneManager.LoadScene(2);
    }
}