using System;
using System.Collections.Generic;
using MyGameDevTools.Extensions;
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

        List<IFocusable> _componentCache = new(2);
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

            ClearTarget();

            enabled = value;
            Cursor.visible = !value;
            _focusCursor.SetPosition(_mainCamera.ViewportToWorldPoint(Vector2.one / 2));
            _focusCursor.SetState(FocusCursor.CursorState.Free);
            if (value)
                this.DelayCallInFrames(1, () => _focusCursor.gameObject.SetActive(value));
            else
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
                    UnselectTarget();
                    return;
                }

                // Drag
                _focusableTarget.OnDrag(inputPosition - _lastInputPosition, inputPosition);
                _lastInputPosition = inputPosition;
            }
            else
            {
                _focusCursor.SetPosition(inputPosition);
                if (Physics2D.OverlapCircle(inputPosition, .1f, new ContactFilter2D
                {
                    useLayerMask = true,
                    layerMask = _interactableMask.value
                }, _circleCastCache) > 0)
                {
                    if (!TryGetFocusable(_circleCastCache[0], out var focusable) || !focusable.Enabled)
                        return;

                    if (focusable == _focusableTarget && Input.GetButtonDown("Fire1"))
                    {
                        focusable.OnSelect();
                        if (focusable.IsDraggable())
                        {
                            _isSelecting = true;
                            _lastInputPosition = inputPosition;
                            _focusCursor.SetState(FocusCursor.CursorState.Selecting);
                        }
                        return;
                    }

                    SetTarget(focusable);
                }
                else if (hasTarget)
                    SetTarget(null);
            }
        }

        void SetTarget(IFocusable focusable)
        {
            if (focusable is null)
            {
                _focusableTarget.Disabled -= ClearTarget;
                _focusableTarget.OnHoverEnd();
                _focusableTarget = null;
                _focusCursor.SetState(FocusCursor.CursorState.Free);
                return;
            }

            _focusableTarget = focusable;
            _focusableTarget.Disabled += ClearTarget;
            _focusableTarget.OnHoverStart();
            _focusCursor.SetState(FocusCursor.CursorState.Hovering);
        }

        void UnselectTarget()
        {
            if (!_isSelecting)
                return;

            _focusableTarget.OnUnselect();
            _isSelecting = false;
            _focusCursor.SetState(FocusCursor.CursorState.Hovering);
        }

        void ClearTarget()
        {
            if (_focusableTarget is null)
                return;

            UnselectTarget();
            SetTarget(null);
        }

        bool TryGetFocusable(Component sourceObject, out IFocusable focusable)
        {
            sourceObject.GetComponents(_componentCache);
            foreach (var focusableComponent in _componentCache)
            {
                if (focusableComponent.Enabled)
                {
                    focusable = focusableComponent;
                    return true;
                }
            }

            focusable = null;
            return false;
        }
    }
}