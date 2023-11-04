using UnityEngine;

namespace Recall.Gameplay
{
    [DisallowMultipleComponent]
    public class WaterDamage : MonoBehaviour
    {
        void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.TryGetComponent<IDamageable>(out var damageable))
                damageable.TakeDamage(int.MaxValue);
        }
    }
}