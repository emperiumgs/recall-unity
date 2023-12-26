using System;
using Recall.Gameplay.Interfaces;
using UnityEngine;

namespace Recall.Gameplay
{
    [Flags, Serializable]
    public enum InputFlags
    {
        None = 0,
        Focus = 0b1,
        Movement = 0b10,
        Combat = 0b100,
        All = Focus | Movement | Combat
    }

    [DisallowMultipleComponent]
    public class InputController : MonoBehaviour
    {
        CharacterController2D _characterController;
        CharacterCombat _characterCombat;
        FocusBehavior _focus;
        InputFlags _inputFlags = InputFlags.All;
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
            if (_inputFlags.HasFlag(InputFlags.Focus) && Input.GetButtonDown("Focus"))
                _focus.ToggleFocus();

            if (_inFocus)
                return;

            if (_inputFlags.HasFlag(InputFlags.Combat))
            {
                if (Input.GetButtonDown("Shield"))
                    _characterCombat.SetDefending(true);
                else if (Input.GetButtonUp("Shield"))
                    _characterCombat.SetDefending(false);

                if (Input.GetButtonDown("Fire1"))
                    _characterCombat.Attack();
            }

            float horizontal = 0;
            bool jump = false;
            if (_inputFlags.HasFlag(InputFlags.Movement))
            {
                horizontal = Input.GetAxis("Horizontal");
                jump = Input.GetButtonDown("Jump");
            }
            _characterController.SetInputs(horizontal, jump);
        }

        public void SetInput(bool active)
        {
            SetInputFlags(active ? InputFlags.All : InputFlags.None);
        }

        public void SetFocusOnlyInput()
        {
            SetInputFlags(InputFlags.Focus);
        }

        public void SetInputFlags(InputFlags flags)
        {
            _inputFlags = flags;
            if (!flags.HasFlag(InputFlags.Focus))
                _inFocus = false;
        }

        void OnRespawned(Vector2 position)
        {
            SetInputFlags(InputFlags.All);
        }

        void OnDamageTaken(int damage)
        {
            SetInputFlags(InputFlags.None);
        }

        void OnFocused(bool isFocused)
        {
            _inFocus = isFocused;
        }

        void OnDeath()
        {
            SetInputFlags(InputFlags.None);
        }

        void OnRecovered()
        {
            SetInputFlags(InputFlags.All);
        }

        void OnOozed(bool isOozed)
        {
            SetInputFlags(isOozed ? InputFlags.None : InputFlags.All);
        }
    }
}