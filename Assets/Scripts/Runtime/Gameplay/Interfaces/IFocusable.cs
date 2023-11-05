using System;
using UnityEngine;

namespace Recall.Gameplay.Interfaces
{
    public interface IFocusable
    {
        event Action Disabled;

        Collider2D Collider { get; }
        bool Enabled { get; }

        void OnHoverStart();

        void OnHoverEnd();

        void OnSelect();

        void OnUnselect();

        void OnDrag(Vector2 delta, Vector2 finalPosition);

        bool IsDraggable();
    }
}