using UnityEngine;
using UnityEngine.Events;

namespace Recall.Gameplay
{
    public class ProgressTrigger : MonoBehaviour
    {
        [SerializeField]
        UnityEvent _onHelicopterDidCrash;
        [SerializeField]
        UnityEvent _onHasNotCard;

        WorldProgressBehavior _progressBehavior;

        void Awake()
        {
            _progressBehavior = FindAnyObjectByType<WorldProgressBehavior>();
        }

        public void Trigger()
        {
            if (_progressBehavior.ProgressFlags.HasFlag(WorldProgressFlags.HelicopterCrashed))
                _onHelicopterDidCrash?.Invoke();

            if (!_progressBehavior.ProgressFlags.HasFlag(WorldProgressFlags.GotAccessCard))
                _onHasNotCard?.Invoke();
        }
    }
}