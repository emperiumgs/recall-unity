using UnityEngine;

namespace Recall.Gameplay
{
    public class HazardBehavior : MonoBehaviour
    {
        [SerializeField, Range(0, 100)]
        int _damage = 10;
        [SerializeField, Range(0, 3)]
        float _cooldown = 1;

        float _lastDamageTimestamp;
        int _targetLayer;

        void Awake()
        {
            _targetLayer = LayerMask.NameToLayer("Player");
        }

        void OnTriggerStay2D(Collider2D collider2D)
        {
            if (collider2D.gameObject.layer != _targetLayer)
                return;

            var currentTimestamp = Time.time;
            if (currentTimestamp > _lastDamageTimestamp + _cooldown)
            {
                if (collider2D.TryGetComponent<IDamageable>(out var damageable))
                    damageable.TakeDamage(_damage);
                _lastDamageTimestamp = currentTimestamp;
            }
        }
    }
}