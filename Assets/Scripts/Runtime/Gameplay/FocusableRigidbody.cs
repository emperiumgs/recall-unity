using System;
using Recall.Gameplay.Interfaces;
using UnityEngine;

namespace Recall.Gameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class FocusableRigidbody : MonoBehaviour, IFocusable
    {
#pragma warning disable 67
        public event Action Disabled;
#pragma warning restore 67

        public Collider2D Collider { get; private set; }
        public bool IsDraggable => true;

        Rigidbody2D _rigidbody;

        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            Collider = GetComponent<Collider2D>();
        }

        public void OnHoverStart()
        {

        }

        public void OnHoverEnd()
        {

        }

        public void OnSelect()
        {
            _rigidbody.gravityScale = 0;
            _rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            _rigidbody.sleepMode = RigidbodySleepMode2D.NeverSleep;
            _rigidbody.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        public void OnUnselect()
        {
            _rigidbody.gravityScale = 1;
            _rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
            _rigidbody.sleepMode = RigidbodySleepMode2D.StartAwake;
            _rigidbody.interpolation = RigidbodyInterpolation2D.None;
        }

        public void OnDrag(Vector2 delta, Vector2 finalPosition)
        {
            _rigidbody.MovePosition(finalPosition);
        }
    }
}