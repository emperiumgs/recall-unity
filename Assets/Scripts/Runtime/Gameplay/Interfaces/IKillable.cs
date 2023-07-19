using System;

namespace Recall.Gameplay.Interfaces
{
    public interface IKillable
    {
        event Action Killed;
    }
}