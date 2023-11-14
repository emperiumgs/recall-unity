using Recall.Gameplay.Interfaces;
using UnityEngine;

namespace Recall.Gameplay
{
    [DisallowMultipleComponent]
    public class InputController : MonoBehaviour
    {
        CharacterController2D _characterController;
        CharacterCombat _characterCombat;
        FocusBehavior _focus;
        bool _inFocus;

        void Awake()
        {
            _characterController = GetComponent<CharacterController2D>();
            _characterCombat = GetComponent<CharacterCombat>();
            _focus = GetComponent<FocusBehavior>();
            _focus.Focused += OnFocused;

            if (TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.DamageTaken += OnDamageTaken;
                damageable.Recovered += OnRecovered;
            }

            if (TryGetComponent<IKillable>(out var killable))
                killable.Killed += OnDeath;

            if (TryGetComponent<IRespawnable>(out var respawnable))
                respawnable.RespawnedAt += OnRespawned;

            if (TryGetComponent<IOozable>(out var oozable))
                oozable.Oozed += OnOozed;
        }

        void Update()
        {
            if (Input.GetButtonDown("Focus"))
                _focus.ToggleFocus();

            if (_inFocus)
                return;

            if (Input.GetButtonDown("Shield"))
                _characterCombat.SetDefending(true);
            else if (Input.GetButtonUp("Shield"))
                _characterCombat.SetDefending(false);

            if (Input.GetButtonDown("Fire1"))
                _characterCombat.Attack();

            var horizontal = Input.GetAxis("Horizontal");
            _characterController.SetInputs(horizontal, Input.GetButtonDown("Jump"));
        }

        public void SetInputActive(bool value)
        {
            enabled = value;
            if (!value)
            {
                _characterController.SetInputs(0, false);
                _inFocus = false;
            }
        }

        void OnRespawned(Vector2 position)
        {
            SetInputActive(true);
        }

        void OnDamageTaken(int damage)
        {
            SetInputActive(false);
        }

        void OnFocused(bool isFocused)
        {
            _inFocus = isFocused;
        }

        void OnDeath()
        {
            SetInputActive(false);
        }

        void OnRecovered()
        {
            SetInputActive(true);
        }

        void OnOozed(bool isOozed)
        {
            SetInputActive(!isOozed);
        }
    }
}