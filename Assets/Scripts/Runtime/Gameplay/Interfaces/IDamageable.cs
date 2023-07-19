using System;

public interface IDamageable
{
    event Action<int> DamageTaken;

    void TakeDamage(int damage);
}