using System;
using UnityEngine;

namespace Recall.Gameplay
{
    public enum CombatState
    {
        Inactive,
        Ready,
        Attacking,
        Defending
    }

    [DisallowMultipleComponent]
    public class CharacterCombat : MonoBehaviour
    {
        public event Action<CombatState> StateChanged;
        public event Action<bool> DefendingStateChanged;
        public event Action<int> ComboAttackStarted;
        public event Action ComboEnded;

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

        CombatState _state = CombatState.Ready;
        CharacterController2D _characterController;
        Collider2D[] _colliderCache = new Collider2D[3];
        int _comboIndex;

        void Awake()
        {
            _characterController = GetComponent<CharacterController2D>();
        }

        public void Attack()
        {
            if (_state != CombatState.Ready)
                return;

            //source.PlayOneShot(Random.Range(0, 2) > 0 ? atkA : atkB);

            _knivesRenderer.enabled = false;
            SetCombatState(CombatState.Attacking);
            ComboAttackStarted?.Invoke(++_comboIndex);
        }

        public void SetDefending(bool value)
        {
            if (value && _state == CombatState.Ready || _state == CombatState.Attacking)
            {
                if (_comboIndex > 0)
                    ClearAttack();

                _shieldObject.SetActive(true);
                _knivesRenderer.enabled = false;

                SetCombatState(CombatState.Defending);
                DefendingStateChanged?.Invoke(value);
            }
            else if (!value)
            {
                _shieldObject.SetActive(false);
                _knivesRenderer.enabled = true;

                SetCombatState(CombatState.Ready);
                DefendingStateChanged?.Invoke(value);
            }
        }

        void DealDamage()
        {
            if (_comboIndex == 0)
                return;

            if (Physics2D.OverlapBoxNonAlloc(GetAttackBoxCenter(), _attackDetectionArea, 0, _colliderCache, _enemyMask) == 0)
                return;

            IDamageable damageable;
            foreach (Collider2D hit in _colliderCache)
            {
                if (hit.TryGetComponent(out damageable))
                {
                    damageable.TakeDamage(_comboDamageList[_comboIndex - 1]);
                    //source.PlayOneShot(atkHit);
                }
            }
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
            EnableAttack();
            _knivesRenderer.enabled = true;
            ComboEnded?.Invoke();
        }

        Vector2 GetAttackBoxCenter()
        {
            var offsetPosition = _attackOffsetPosition;
            if (_characterController.IsFlipped)
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