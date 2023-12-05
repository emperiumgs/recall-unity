using MyGameDevTools.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace Recall.Gameplay
{
    public class ActionTrigger : MonoBehaviour
    {
        [SerializeField]
        LayerMask _targetMask;
        [SerializeField]
        UnityEvent _onAction;

        Collider2D _collider;

        void OnEnable()
        {
            _collider = GetComponent<Collider2D>();
        }

        void OnTriggerEnter2D(Collider2D collider)
        {
            if (_targetMask.HasLayer(collider.gameObject.layer))
            {
                _onAction?.Invoke();
                _collider.enabled = false;
            }
        }
    }
}