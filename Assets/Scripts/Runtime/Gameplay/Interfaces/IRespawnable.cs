using System;
using UnityEngine;

namespace Recall.Gameplay.Interfaces
{
    public interface IRespawnable
    {
        event Action<Vector2> RespawnedAt;

        void SetRespawnPosition(Vector2 respawnPosition);

        void Respawn();
    }
}