using UnityEngine;
using UnityEngine.Events;

namespace Recall.Gameplay
{
    public class SectorBehavior : MonoBehaviour
    {
        [SerializeField]
        GameObject[] _enemies;
        [SerializeField]
        UnityEvent _onCleared;

        void FixedUpdate()
        {
            foreach (var enemy in _enemies)
                if (enemy.activeSelf)
                    return;

            _onCleared?.Invoke();
            enabled = false;
        }
    }
}