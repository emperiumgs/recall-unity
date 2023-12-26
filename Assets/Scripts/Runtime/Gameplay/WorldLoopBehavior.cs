using System;
using UnityEngine;

namespace Recall.Gameplay
{
    public class WorldLoopBehavior : MonoBehaviour
    {
        public event Action<int> WorldLoop;

        public bool IsFirstLoop => LoopIndex == _firstLoop;
        public int LoopIndex { get; private set; }

        const int _firstLoop = 0;

#if UNITY_EDITOR
        [SerializeField, Range(-1, 2)]
        int _loopIndexOverride = -1;
#endif

        void Awake()
        {
#if UNITY_EDITOR
            LoopIndex = _loopIndexOverride >= 0 ? _loopIndexOverride : _firstLoop;
#else
            LoopIndex = _firstLoop;
#endif
        }

        public void TriggerWorldLoop()
        {
            LoopIndex++;
            WorldLoop?.Invoke(LoopIndex);
        }
    }
}