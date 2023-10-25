using System;
using UnityEngine;

namespace Recall.Gameplay
{
    [DisallowMultipleComponent]
    public class FocusBehavior : MonoBehaviour
    {
        public event Action<bool> Focused;

        FocusSystem _focusSystem;

        void Awake()
        {
            _focusSystem = FindAnyObjectByType<FocusSystem>();
        }

        public void ToggleFocus()
        {
            SetFocus(!_focusSystem.FocusActive);
        }

        public void SetFocus(bool value)
        {
            _focusSystem.SetFocus(value);
            Focused?.Invoke(value);
        }
    }
}