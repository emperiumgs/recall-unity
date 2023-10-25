using System;
using Recall.Gameplay.Interfaces;
using UnityEngine;

namespace Recall.Gameplay
{
    [DisallowMultipleComponent]
    public class FocusSystem : MonoBehaviour
    {
        public event Action<bool> FocusModeChanged;

        public bool FocusActive => enabled;

        [SerializeField]
        LayerMask _interactableMask;

        IFocusable _focusableTarget;
        Collider2D[] _circleCastCache = new Collider2D[1];
        GameObject _focusCursor;
        Camera _mainCamera;
        Vector2 _lastInputPosition;
        bool _isSelecting;

        void Awake()
        {
            _focusCursor = new GameObject("Cursor_Focus");
            _mainCamera = Camera.main;
        }

        public void SetFocus(bool value)
        {
            if (FocusActive == value)
                return;

            enabled = value;
            FocusModeChanged?.Invoke(value);
        }

        void Update()
        {
            bool hasTarget = _focusableTarget is not null;

            // Set focus cursor position
            var convertedPosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            var inputPosition = new Vector2(convertedPosition.x, convertedPosition.y);

            if (hasTarget && _isSelecting)
            {
                _focusCursor.transform.position = _focusableTarget.Collider.bounds.center;
                // Release
                if (Input.GetButtonUp("Fire1"))
                {
                    _focusableTarget.OnUnselect();
                    _isSelecting = false;
                    // display feedback on cursor
                    return;
                }

                // Drag
                _focusableTarget.OnDrag(inputPosition - _lastInputPosition, inputPosition);
                _lastInputPosition = inputPosition;
            }
            else
            {
                _focusCursor.transform.position = inputPosition;
                // Perform raycasts
                if (Physics2D.OverlapCircleNonAlloc(inputPosition, .1f, _circleCastCache, _interactableMask) > 0)
                {
                    if (!_circleCastCache[0].TryGetComponent<IFocusable>(out var focusable))
                        return;

                    if (focusable == _focusableTarget && Input.GetButtonDown("Fire1"))
                    {
                        focusable.OnSelect();
                        _isSelecting = true;
                        _lastInputPosition = inputPosition;
                        // display feedback on cursor
                        return;
                    }

                    focusable.OnHoverStart();
                    _focusableTarget = focusable;
                    // display feedback on cursor
                }
                else if (hasTarget)
                {
                    _focusableTarget.OnHoverEnd();
                    _focusableTarget = null;
                    // display feedback on cursor
                }
            }
        }
    }
}