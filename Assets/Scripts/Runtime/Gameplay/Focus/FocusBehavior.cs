using System;
using UnityEngine;

namespace Recall.Gameplay
{
    [DisallowMultipleComponent]
    public class FocusBehavior : MonoBehaviour
    {
        public event Action<bool> Focused;

        [SerializeField]
        GameObject _focusMask;

        FocusSystem _focusSystem;

        void Awake()
        {
            _focusSystem = FindAnyObjectByType<FocusSystem>();

            _focusMask.transform.SetParent(null, true);
            _focusMask.SetActive(false);
        }

        public void ToggleFocus()
        {
            SetFocus(!_focusSystem.FocusActive);
        }

        public void SetFocus(bool value)
        {
            _focusSystem.SetFocus(value);
            _focusMask.SetActive(value);
            Focused?.Invoke(value);
        }
    }
}