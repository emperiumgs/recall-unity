using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Recall.Gameplay
{
    public class FocusBehavior : MonoBehaviour
    {
        public event Action<bool> Focused;

        Physics2DRaycaster _raycaster;
        bool _focused;

        void Awake()
        {
            _raycaster = FindAnyObjectByType<Physics2DRaycaster>();
        }

        public void ToggleFocus()
        {
            SetFocus(!_focused);
        }

        public void SetFocus(bool value)
        {
            _focused = value;
            _raycaster.enabled = value;
            Focused?.Invoke(value);
        }
    }
}