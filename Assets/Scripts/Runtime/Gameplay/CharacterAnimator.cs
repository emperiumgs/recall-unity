using Recall.Gameplay.Interfaces;
using UnityEngine;
using UnityEngine.Windows;

namespace Recall.Gameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public class CharacterAnimator : MonoBehaviour
    {
        CharacterController2D _characterController;
        CharacterCombat _characterCombat;
        Animator _animator;
        int _walkingHash;
        int _triggerHash;
        int _attackHash;
        int _shieldHash;
        int _damageHash;
        int _deathHash;
        int _jumpHash;

        void Awake()
        {
            _characterController = GetComponent<CharacterController2D>();
            _characterController.GroundedStateChanged += OnGroundedStateChanged;
            _characterController.Jumped += OnJump;

            _characterCombat = GetComponent<CharacterCombat>();
            _characterCombat.DefendingStateChanged += OnDefendingStateChanged;
            _characterCombat.ComboAttackStarted += OnComboAttackStarted;
            _characterCombat.ComboEnded += OnComboEnded;

            if (TryGetComponent<IDamageable>(out var damageable))
                damageable.DamageTaken += OnDamageTaken;

            if (TryGetComponent<IKillable>(out var killable))
                killable.Killed += OnDeath;

            if (TryGetComponent<IRespawnable>(out var respawnable))
                respawnable.RespawnedAt += OnRespawned;

            _animator = GetComponent<Animator>();

            _walkingHash = Animator.StringToHash("walking");
            _triggerHash = Animator.StringToHash("trigger");
            _shieldHash = Animator.StringToHash("shield");
            _damageHash = Animator.StringToHash("damage");
            _attackHash = Animator.StringToHash("atkNum");
            _deathHash = Animator.StringToHash("death");
            _jumpHash = Animator.StringToHash("jump");
        }

        public void SetHorizontalInput(float input)
        {
            _animator.SetBool(_walkingHash, input != 0);
        }

        void OnDamageTaken(int damage)
        {
            _animator.SetTrigger(_damageHash);
        }

        void OnRespawned(Vector2 position)
        {
            _animator.SetBool(_deathHash, false);
        }

        void OnDeath()
        {
            _animator.SetBool(_walkingHash, false);
            _animator.SetBool(_shieldHash, false);
            _animator.SetBool(_jumpHash, false);
            _animator.SetBool(_deathHash, true);
            _animator.SetInteger(_attackHash, 0);
            _animator.SetTrigger(_triggerHash);
        }

        void OnJump()
        {
            _animator.SetBool(_jumpHash, true);
        }

        void OnGroundedStateChanged(bool isGrounded)
        {
            _animator.SetBool(_jumpHash, !isGrounded);
        }

        void OnDefendingStateChanged(bool isDefending)
        {
            if (isDefending)
            {
                _animator.SetBool(_shieldHash, true);
                _animator.SetTrigger(_triggerHash);
            }
            else
            {
                _animator.SetBool(_shieldHash, false);
            }
        }

        void OnComboAttackStarted(int comboIndex)
        {
            _animator.SetInteger(_attackHash, comboIndex);
        }

        void OnComboEnded()
        {
            _animator.SetInteger(_attackHash, 0);
        }
    }
}