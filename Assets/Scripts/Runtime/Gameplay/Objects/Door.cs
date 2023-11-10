using System.Collections;
using UnityEngine;

namespace Recall.Gameplay
{
    public class Door : MonoBehaviour
    {
        [SerializeField]
        Vector2 _openPosition = new(0, 1.5f);
        [SerializeField]
        Vector2 _moveDestination = new(.6f, 0);
        [SerializeField]
        bool _openByTrigger = true;
        [SerializeField, Range(.1f, .5f)]
        float _animationTime = .25f;
        [SerializeField]
        AnimationCurve _openCurve;
        [SerializeField]
        AnimationCurve _closeCurve;

        BoxCollider2D _doorCollider;
        BoxCollider2D _doorTrigger;
        bool _isOpen;

        void Awake()
        {
            SetReferences();
        }

        void OnEnable()
        {
            if (!_openByTrigger)
                _doorTrigger.enabled = false;
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

        }
        
        public void Open()
        {
            if (_isOpen)
                return;

            // add oncomplete callback
            TransitionToState(_openPosition, _openCurve);
            _isOpen = true;
        }

        public void Close()
        {
            if (!_isOpen)
                return;

            TransitionToState(Vector2.zero, _closeCurve);
        }

        void ResolveState(bool isOpen)
        {
            _doorCollider.transform.position = isOpen ? _openPosition : Vector2.zero;
        }

        void SetReferences()
        {
            _doorTrigger = GetComponent<BoxCollider2D>();
            _doorCollider = transform.GetChild(0).GetComponent<BoxCollider2D>();
        }

        Coroutine TransitionToState(Vector2 targetState, AnimationCurve curve)
        {
            return StartCoroutine(transitionRoutine());
            IEnumerator transitionRoutine()
            {
                var endOfFrame = new WaitForEndOfFrame();
                var initialState = (Vector2)transform.position;
                float time = 0;
                while (time < _animationTime)
                {
                    time += Time.deltaTime;
                    transform.position = Vector2.Lerp(initialState, targetState, curve.Evaluate(time / _animationTime));
                    yield return endOfFrame;
                }
            }
        }
    }
}