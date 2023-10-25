using UnityEngine;

namespace Recall.Gameplay.Interfaces
{
    public interface IFocusable
    {
        Collider2D Collider { get; }

        void OnHoverStart();

        void OnHoverEnd();

        void OnSelect();

        void OnUnselect();

        void OnDrag(Vector2 delta, Vector2 finalPosition);
    }
}