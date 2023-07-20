using Recall.Gameplay.Interfaces;
using System;
using System.Collections;
using UnityEngine;

namespace Recall.Gameplay
{
    [DisallowMultipleComponent]
    public class CharacterOozer : MonoBehaviour, IOozable
    {
        public event Action<bool> Oozed;

        [SerializeField, Range(.5f, 3)]
        float _oozeDuration = 2f;

        Coroutine _oozeRoutine;

        void Awake()
        {
            if (TryGetComponent<IKillable>(out var killable))
                killable.Killed += OnDeath;
        }

        public void ApplyOoze()
        {
            if (_oozeRoutine is not null)
                StopCoroutine(_oozeRoutine);
            _oozeRoutine = OozeTimer();

            Oozed?.Invoke(true);
        }

        void UnapplyOoze()
        {
            Oozed?.Invoke(false);
            _oozeRoutine = null;
        }

        void OnDeath()
        {
            if (_oozeRoutine is not null)
            {
                StopCoroutine(_oozeRoutine);
                UnapplyOoze();
            }
        }

        Coroutine OozeTimer()
        {
            return StartCoroutine(oozeTimerRoutine());
            IEnumerator oozeTimerRoutine()
            {
                yield return new WaitForSeconds(_oozeDuration);
                UnapplyOoze();
            }
        }
    }
}