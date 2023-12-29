using System;
using UnityEngine;

namespace Recall.Gameplay
{
    [Flags, Serializable]
    public enum WorldProgressFlags : byte
    {
        None = 0,
        HelicopterCrashed = 0b1,
        GotAccessCard = 0b10,
        All = HelicopterCrashed | GotAccessCard
    }

    public class WorldProgressBehavior : MonoBehaviour
    {
        public event Action<WorldProgressFlags> ProgressChanged;

        public WorldProgressFlags ProgressFlags { get; private set; }

        public void OnHelicopterCrashed() => SetProgress(ProgressFlags | WorldProgressFlags.HelicopterCrashed);

        public void OnGetCard() => SetProgress(ProgressFlags | WorldProgressFlags.GotAccessCard);

        void SetProgress(WorldProgressFlags progressFlags)
        {
            ProgressFlags = progressFlags;
            ProgressChanged?.Invoke(ProgressFlags);
        }
    }
}