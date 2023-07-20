using System;

namespace Recall.Gameplay.Interfaces
{
    public interface IOozable
    {
        event Action<bool> Oozed;

        void ApplyOoze();
    }
}