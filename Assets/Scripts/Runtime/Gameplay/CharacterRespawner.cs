using Recall.Gameplay.Interfaces;
using System;
using UnityEngine;

namespace Recall.Gameplay
{
    public class CharacterRespawner : MonoBehaviour, IRespawnable
    {
        public event Action<Vector2> RespawnedAt;

        Vector2 _respawnPosition;

        void Awake()
        {
            SetRespawnPosition(transform.position);
        }

        public void SetRespawnPosition(Vector2 respawnPosition)
        {
            _respawnPosition = respawnPosition;
        }

        public void Respawn()
        {
            RespawnedAt?.Invoke(_respawnPosition);
        }
    }
}