using UnityEngine;
using UnityEngine.Events;

namespace Recall.Gameplay
{
    public class LoopTrigger : MonoBehaviour
    {
        [SerializeField]
        UnityEvent _onFirstLoop;
        [SerializeField]
        UnityEvent _onNextLoops;

        WorldLoopBehavior _loopBehavior;

        void Awake()
        {
            _loopBehavior = FindAnyObjectByType<WorldLoopBehavior>();
        }

        public void Trigger()
        {
            var eventTrigger = _loopBehavior.IsFirstLoop ? _onFirstLoop : _onNextLoops;
            eventTrigger?.Invoke();
        }
    }
}