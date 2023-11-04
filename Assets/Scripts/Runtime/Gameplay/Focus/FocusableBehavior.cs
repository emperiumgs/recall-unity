using UnityEngine;
using UnityEngine.Events;

namespace Recall.Gameplay
{
    public class FocusableBehavior : FocusableBase
    {
        [SerializeField]
        UnityEvent _selected;

        public override void OnSelect()
        {
            enabled = false;
            _selected?.Invoke();
        }

        public override bool IsDraggable() => false;
    }
}