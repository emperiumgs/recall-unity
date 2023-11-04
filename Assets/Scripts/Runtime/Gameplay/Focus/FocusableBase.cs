using System;
using Recall.Gameplay.Interfaces;
using UnityEngine;

namespace Recall.Gameplay
{
    public abstract class FocusableBase : MonoBehaviour, IFocusable
    {
        public event Action Disabled;

        public Collider2D Collider { get; private set; }
        public bool Enabled => enabled;

        [SerializeField]
        GameObject _particleFeedback;

        FocusSystem _focusSystem;

        protected virtual void Awake()
        {
            Collider = GetComponent<Collider2D>();

            _focusSystem = FindAnyObjectByType<FocusSystem>();
        }

        void OnEnable()
        {
            _focusSystem.FocusModeChanged += OnFocusModeChanged;
            OnFocusModeChanged(_focusSystem.FocusActive);
        }

        void OnDisable()
        {
            Disabled?.Invoke();
            OnFocusModeChanged(false);

            if (!_focusSystem)
                return;

            _focusSystem.FocusModeChanged -= OnFocusModeChanged;
        }

        public virtual void OnHoverStart() {}

        public virtual void OnHoverEnd() {}

        public virtual void OnSelect()
        {
            _particleFeedback.SetActive(false);
        }

        public virtual void OnUnselect()
        {
            _particleFeedback.SetActive(true);
        }

        public virtual void OnDrag(Vector2 delta, Vector2 finalPosition) {}

        public virtual bool IsDraggable() => true;

        void OnFocusModeChanged(bool focusActive)
        {
            _particleFeedback.SetActive(focusActive);
        }
    }
}