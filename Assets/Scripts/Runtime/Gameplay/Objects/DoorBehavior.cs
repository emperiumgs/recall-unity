using System;
using System.Collections;
using MyGameDevTools.Extensions;
using UnityEngine;

namespace Recall.Gameplay
{
    public class DoorBehavior : MonoBehaviour
    {
        [SerializeField]
        Vector2 _openPosition = new(0, 1.5f);
        [SerializeField]
        Vector2 _moveDestination = new(.6f, 0);
        [SerializeField]
        bool _locked = false;
        [SerializeField, Range(.1f, 1.5f)]
        float _animationTime = .25f;
        [SerializeField]
        AnimationCurve _openCurve;
        [SerializeField]
        AnimationCurve _closeCurve;
        [SerializeField, Range(0, .5f)]
        float _regainInputGraceTime = .2f;
        [SerializeField]
        bool _startOpen;

        CharacterMoveCommand _cachedMoveCommand;
        BoxCollider2D _doorCollider;
        BoxCollider2D _doorTrigger;
        Transform _doorTransform;
        bool _isOpen;
        int _playerLayer;

        void Awake()
        {
            SetReferences();
        }

        void OnEnable()
        {
            _doorTrigger.enabled = !_locked;
        }

        void OnDrawGizmosSelected()
        {
            if (!_doorCollider)
                SetReferences();

            var color = Gizmos.color;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube((Vector2)transform.position + _openPosition + _doorCollider.offset, _doorCollider.size);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube((Vector2)transform.position + _moveDestination + new Vector2(0, 0.7f), new Vector2(.5f, 1.4f));
            Gizmos.color = color;
        }

        void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject.layer != _playerLayer)
                return;

            collider.GetComponent<InputController>().SetInputActive(false);
            _cachedMoveCommand = collider.GetComponent<CharacterMoveCommand>();
            _cachedMoveCommand.MoveCompleted += OnCharacterMoveCompleted;

            if (!_isOpen)
                Open(CommandCharacterMove);
            else
                CommandCharacterMove();
        }
        
        public void Open(Action onComplete)
        {
            if (_isOpen)
                return;

            TransitionToState(_openPosition, _openCurve, onComplete);
            _isOpen = true;
        }

        public void OpenAndUnlock()
        {
            Unlock();
            Open(null);
        }

        public void Close()
        {
            if (!_isOpen)
                return;

            TransitionToState(Vector2.zero, _closeCurve, null);
            _isOpen = false;
        }

        public void Unlock()
        {
            _locked = false;
            _doorTrigger.enabled = true;
        }

        void ResolveState(bool isOpen)
        {
            _doorTransform.localPosition = isOpen ? _openPosition : Vector2.zero;
            _isOpen = isOpen;
        }

        void SetReferences()
        {
            _playerLayer = LayerMask.NameToLayer("Player");
            _doorTrigger = GetComponent<BoxCollider2D>();
            _doorCollider = transform.GetChild(0).GetComponent<BoxCollider2D>();
            _doorTransform = _doorCollider.transform;

            ResolveState(_startOpen);
        }

        void CommandCharacterMove()
        {
            _cachedMoveCommand.MoveToPosition((Vector2)transform.position + _moveDestination);
        }

        void OnCharacterMoveCompleted()
        {
            _cachedMoveCommand.MoveCompleted -= OnCharacterMoveCompleted;

            Close();
            this.DelayCall(_regainInputGraceTime, () => _cachedMoveCommand.GetComponent<InputController>().SetInputActive(true));
        }

        Coroutine TransitionToState(Vector2 targetState, AnimationCurve curve, Action onComplete)
        {
            return StartCoroutine(transitionRoutine());
            IEnumerator transitionRoutine()
            {
                var endOfFrame = new WaitForEndOfFrame();
                var initialState = (Vector2)_doorTransform.localPosition;
                float time = 0;
                while (time < _animationTime)
                {
                    time += Time.deltaTime;
                    _doorTransform.localPosition = Vector2.Lerp(initialState, targetState, curve.Evaluate(time / _animationTime));
                    yield return endOfFrame;
                }

                onComplete?.Invoke();
            }
        }
    }
}