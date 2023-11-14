using System;
using UnityEngine;

namespace Recall.Gameplay
{
    public class PressurePlateBehavior : MonoBehaviour
    {
        public event Action<bool> StateChanged;

        public bool IsPressed { get; private set; }

        [SerializeField]
        Transform _physicsTransform;
        [SerializeField, Range(.025f, .25f)]
        float _isPressedThreshold = .1f;

        void FixedUpdate()
        {
            SetState(_physicsTransform.localPosition.y <= -_isPressedThreshold);
        }

        public void SetState(bool value)
        {
            if (IsPressed == value)
                return;

            IsPressed = value;
            StateChanged?.Invoke(IsPressed);
        }
    }
}