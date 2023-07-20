using Recall.Gameplay.Interfaces;
using UnityEngine;

namespace Recall.Gameplay
{
    [DisallowMultipleComponent]
    public class InputController : MonoBehaviour
    {
        CharacterController2D _characterController;
        CharacterAnimator _characterAnimator;
        CharacterCombat _characterCombat;

        void Awake()
        {
            _characterController = GetComponent<CharacterController2D>();
            _characterAnimator = GetComponent<CharacterAnimator>();
            _characterCombat = GetComponent<CharacterCombat>();

            if (TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.DamageTaken += OnDamageTaken;
                damageable.Recovered += OnRecovered;
            }

            if (TryGetComponent<IKillable>(out var killable))
                killable.Killed += OnDeath;

            if (TryGetComponent<IRespawnable>(out var respawnable))
                respawnable.RespawnedAt += OnRespawned;
        }

        void Update()
        {
            if (Input.GetButtonDown("Shield"))
                _characterCombat.SetDefending(true);
            else if (Input.GetButtonUp("Shield"))
                _characterCombat.SetDefending(false);

            if (Input.GetButtonDown("Fire1"))
                _characterCombat.Attack();

            var horizontal = Input.GetAxis("Horizontal");
            _characterController.SetInputs(horizontal, Input.GetButtonDown("Jump"));
            _characterAnimator.SetHorizontalInput(horizontal);
        }

        public void SetInputActive(bool value)
        {
            enabled = value;
        }

        void OnRespawned(Vector2 position)
        {
            SetInputActive(true);
        }

        void OnDamageTaken(int damage)
        {
            SetInputActive(false);
        }

        void OnDeath()
        {
            SetInputActive(false);
        }

        void OnRecovered()
        {
            SetInputActive(true);
        }
    }
}