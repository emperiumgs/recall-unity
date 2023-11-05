using Recall.Gameplay.Interfaces;
using System;
using System.Collections;
using UnityEngine;

namespace Recall.Gameplay
{
    public enum CombatState
    {
        Inactive,
        Ready,
        Attacking,
        Defending,
        CoolingDown
    }

    [DisallowMultipleComponent]
    public class CharacterCombat : MonoBehaviour, IBlockable
    {
        public event Action<CombatState> StateChanged;
        public event Action<bool> DefendingStateChanged;
        public event Action<int> ComboAttackStarted;
        public event Action BlockedAttack;
        public event Action ComboEnded;
        public event Action EnemyHit;

        public CombatState State => _state;

        [SerializeField]
        SpriteRenderer _knivesRenderer;
        [SerializeField]
        GameObject _shieldObject;
        [SerializeField]
        LayerMask _enemyMask;
        [SerializeField]
        int[] _comboDamageList = new int[3];
        [SerializeField]
        Vector2 _attackOffsetPosition;
        [SerializeField]
        Vector2 _attackDetectionArea;
        [SerializeField, Range(.1f, 1)]
        float _attackCooldown = .5f;

        CombatState _state = CombatState.Ready;
        ISpriteFlippable _flippable;
        Collider2D[] _colliderCache = new Collider2D[3];
        Coroutine _cooldownRoutine;
        int _comboIndex;

        void Awake()
        {
            _flippable = GetComponent<ISpriteFlippable>();

            if (TryGetComponent<IDamageable>(out var damageable))
                damageable.DamageTaken += OnDamageTaken;

            if (TryGetComponent<IKillable>(out var killable))
                killable.Killed += ResetCombatState;

            if (TryGetComponent<IRespawnable>(out var respawnable))
                respawnable.RespawnedAt += OnRespawned;

            if (TryGetComponent<IOozable>(out var oozable))
                oozable.Oozed += OnImmovableState;

            if (TryGetComponent<FocusBehavior>(out var focus))
                focus.Focused += OnImmovableState;
        }

        public void Attack()
        {
            if (_state != CombatState.Ready)
                return;

            SetKnifeRenderer(false);
            SetCombatState(CombatState.Attacking);
            ComboAttackStarted?.Invoke(++_comboIndex);
        }

        public void SetDefending(bool value)
        {
            if (value && _state == CombatState.Ready || _state == CombatState.Attacking || _state == CombatState.CoolingDown)
            {
                if (_comboIndex > 0)
                    ClearAttack();

                _shieldObject.SetActive(true);
                SetKnifeRenderer(false);

                SetCombatState(CombatState.Defending);
                DefendingStateChanged?.Invoke(value);
            }
            else if (!value)
            {
                _shieldObject.SetActive(false);
                SetKnifeRenderer(true);

                SetCombatState(_cooldownRoutine == null ? CombatState.Ready : CombatState.CoolingDown);
                DefendingStateChanged?.Invoke(value);
            }
        }

        public void Block()
        {
            BlockedAttack?.Invoke();
        }

        public void DisableKnifeRenderer() => SetKnifeRenderer(false);

        void DealDamage()
        {
            if (_comboIndex == 0)
                return;

            if (Physics2D.OverlapBoxNonAlloc(GetAttackBoxCenter(), _attackDetectionArea, 0, _colliderCache, _enemyMask) == 0)
                return;

            IDamageable damageable;
            foreach (Collider2D hit in _colliderCache)
            {
                if (hit && hit.TryGetComponent(out damageable))
                {
                    damageable.TakeDamage(_comboDamageList[_comboIndex - 1]);
                    EnemyHit?.Invoke();
                }
            }
        }

        void SetKnifeRenderer(bool value)
        {
            _knivesRenderer.enabled = value;
        }

        void EnableAttack()
        {
            SetCombatState(CombatState.Ready);
        }

        void SetCombatState(CombatState state)
        {
            _state = state;
            StateChanged?.Invoke(state);
        }

        void ClearAttack()
        {
            _comboIndex = 0;
            SetKnifeRenderer(true);

            if (_cooldownRoutine is not null)
                StopCoroutine(_cooldownRoutine);
            _cooldownRoutine = Cooldown();

            ComboEnded?.Invoke();
        }

        void ResetCombatState()
        {
            if (_state == CombatState.Attacking)
                ClearAttack();
            else if (_state == CombatState.Defending)
                SetDefending(false);
        }

        void OnDamageTaken(int damage) => ResetCombatState();

        void OnRespawned(Vector2 position)
        {
            SetKnifeRenderer(true);
        }

        void OnImmovableState(bool stateActive)
        {
            if (stateActive)
                ResetCombatState();
        }

        Coroutine Cooldown()
        {
            return StartCoroutine(cooldownRoutine());
            IEnumerator cooldownRoutine()
            {
                SetCombatState(CombatState.CoolingDown);
                yield return new WaitForSeconds(_attackCooldown);
                EnableAttack();
                _cooldownRoutine = null;
            }
        }

        Vector2 GetAttackBoxCenter()
        {
            var offsetPosition = _attackOffsetPosition;
            if (_flippable.IsFlipped)
                offsetPosition.x *= -1;
            return (Vector2)transform.position + offsetPosition;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + (Vector3)_attackOffsetPosition, _attackDetectionArea);
        }
    }
}