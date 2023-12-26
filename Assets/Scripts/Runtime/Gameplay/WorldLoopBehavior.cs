using System;
using UnityEngine;

namespace Recall.Gameplay
{
    public class WorldLoopBehavior : MonoBehaviour
    {
        public event Action<int> WorldLoop;

        public int LoopIndex { get; private set; }

#if UNITY_EDITOR
        [SerializeField, Range(-1, 2)]
        int _loopIndexOverride = -1;
#endif

        void Awake()
        {
#if UNITY_EDITOR
            LoopIndex = _loopIndexOverride;
#endif
        }

        public void TriggerWorldLoop()
        {
            LoopIndex++;
            WorldLoop?.Invoke(LoopIndex);
        }
    }
}