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
        GameObject _focusCursorPrefab;
        [SerializeField]
        LayerMask _interactableMask;

        IFocusable _focusableTarget;
        Collider2D[] _circleCastCache = new Collider2D[1];
        FocusCursor _focusCursor;
        Camera _mainCamera;
        Vector2 _lastInputPosition;
        bool _isSelecting;

        void Awake()
        {
            _focusCursor = Instantiate(_focusCursorPrefab).GetComponent<FocusCursor>();
            _focusCursor.gameObject.SetActive(false);
            _mainCamera = Camera.main;
        }

        public void SetFocus(bool value)
        {
            if (FocusActive == value)
                return;

            if (_focusableTarget is not null)
            {
                if (_isSelecting)
                {
                    _focusableTarget.OnUnselect();
                    _isSelecting = false;
                }

                _focusableTarget.OnHoverEnd();
                _focusableTarget = null;
            }

            enabled = value;
            Cursor.visible = !value;
            _focusCursor.SetPosition(_mainCamera.ViewportToWorldPoint(Vector2.one / 2));
            _focusCursor.SetState(FocusCursor.CursorState.Free);
            _focusCursor.gameObject.SetActive(value);
            FocusModeChanged?.Invoke(value);
        }

        void Update()
        {
            bool hasTarget = _focusableTarget is not null;

            // Set focus cursor position
            Vector2 inputPosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);

            if (hasTarget && _isSelecting)
            {
                _focusCursor.SetPosition(_focusableTarget.Collider.bounds.center);
                // Release
                if (Input.GetButtonUp("Fire1"))
                {
                    _focusableTarget.OnUnselect();
                    _isSelecting = false;
                    _focusCursor.SetState(FocusCursor.CursorState.Hovering);
                    return;
                }

                // Drag
                _focusableTarget.OnDrag(inputPosition - _lastInputPosition, inputPosition);
                _lastInputPosition = inputPosition;
            }
            else
            {
                _focusCursor.SetPosition(inputPosition);
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
                        _focusCursor.SetState(FocusCursor.CursorState.Selecting);
                        return;
                    }

                    focusable.OnHoverStart();
                    _focusableTarget = focusable;
                    _focusCursor.SetState(FocusCursor.CursorState.Hovering);
                }
                else if (hasTarget)
                {
                    _focusableTarget.OnHoverEnd();
                    _focusableTarget = null;
                    _focusCursor.SetState(FocusCursor.CursorState.Free);
                }
            }
        }
    }
}