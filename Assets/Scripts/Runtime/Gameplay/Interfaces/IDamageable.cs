using System;

public interface IDamageable
{
    event Action<int> DamageTaken;
    event Action Recovered;

    void TakeDamage(int damage);
}