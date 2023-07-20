using Recall.Gameplay.Interfaces;
using System;
using System.Collections;
using UnityEngine;

namespace Recall.Gameplay
{
    public class CharacterHealth : MonoBehaviour, IDamageable, IKillable
    {
        public event Action<int> DamageTaken;
        public event Action Recovered;
        public event Action Killed;

        public int MaxHealth => _maxHealth;
        public int Health => _health;

        [SerializeField, Range(30, 200)]
        int _maxHealth = 100;
        [SerializeField, Range(.5f, 5)]
        float _respawnTime = 2;
        [SerializeField]
        bool _autoRespawn;

        IRespawnable _respawnable;
        int _health;

        void Awake()
        {
            _respawnable = GetComponent<IRespawnable>();
            _health = _maxHealth;
        }

        public void TakeDamage(int damage)
        {
            if (!enabled)
                return;

            _health -= damage;

            if (_health <= 0)
            {
                enabled = false;
                if (_autoRespawn)
                    Respawn();
                Killed?.Invoke();
            }
            else
                DamageTaken?.Invoke(damage);
        }

        public void EndDamage()
        {
            Recovered?.Invoke();
        }

        Coroutine Respawn()
        {
            return StartCoroutine(respawnRoutine());
            IEnumerator respawnRoutine()
            {
                yield return new WaitForSeconds(_respawnTime);
                _respawnable.Respawn();
                _health = _maxHealth;
                enabled = true;
            }
        }
    }
}