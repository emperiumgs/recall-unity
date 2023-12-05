using Recall.Gameplay.Interfaces;
using UnityEngine;

namespace Recall.Gameplay
{
    public class CheckpointBehavior : MonoBehaviour
    {
        void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.TryGetComponent<IRespawnable>(out var respawnable))
                respawnable.SetRespawnPosition(transform.position);
        }
    }
}