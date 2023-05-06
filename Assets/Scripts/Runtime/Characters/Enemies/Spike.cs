using UnityEngine;
using System.Collections;

public class Spike : EnemyChaser
{
    [Range(0.1f, 0.99f)]
    public float shardChance;
    public Bullet[] shards;

    Coroutine shardAttack;
    Vector2[] directions = { Vector2.left, new Vector2(-.5f, .5f), Vector2.up, new Vector2(.5f, .5f), Vector2.right };

    protected override void Awake()
    {
        base.Awake();
        foreach (Bullet b in shards)
            b.damage = damage;
    }

    protected override void Chase()
    {
        base.Chase();
        ShardChance();
    }

    protected override void Engage()
    {
        base.Engage();
        ShardChance();
    }

    void ShardChance()
    {
        if (attackable && shardAttack == null)
            shardAttack = StartCoroutine(ShardAttack());
    }

    void StartSpecial()
    {
        attackable = false;
        anim.SetTrigger("special");
        currentState = null;
    }

    void EndSpecial()
    {
        currentState = Chase;
        StartCoroutine(AttackCooldown());
    }

    void FireShards()
    {
        for (int i = 0; i < shards.Length; i++)
            shards[i].Fire(directions[i]);
    }

    IEnumerator ShardAttack()
    {
        while (attackable && (currentState == Engage || currentState == Chase))
        {
            if (Random.Range(0, 1f) <= shardChance)
                StartSpecial();
            yield return new WaitForSeconds(1);
        }
        shardAttack = null;
    }
}
