using Recall.Gameplay.Interfaces;
using System;
using UnityEngine;

namespace Recall.Gameplay
{
    public class CharacterHealth : MonoBehaviour, IDamageable, IKillable
    {
        public event Action<int> DamageTaken;
        public event Action Killed;

        public int MaxHealth => _maxHealth;
        public int Health => _health;

        [SerializeField, Range(30, 200)]
        int _maxHealth = 100;

        int _health;

        void Awake()
        {
            _health = _maxHealth;
        }

        public void TakeDamage(int damage)
        {
            _health -= damage;

            if (_health <= 0)
                Killed?.Invoke();
            else
                DamageTaken?.Invoke(damage);
        }
    }
}