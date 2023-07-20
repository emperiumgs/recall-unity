using System;

namespace Recall.Gameplay.Interfaces
{
    public interface ISpriteFlippable
    {
        event Action<bool> FlippedStateChanged;

        bool IsFlipped { get; }
    }
}