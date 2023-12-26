using System;
using UnityEngine;

namespace Recall.Gameplay
{
    [DisallowMultipleComponent]
    public class CharacterMoveCommand : MonoBehaviour
    {
        public event Action MoveCompleted;

        [SerializeField, Range(.01f, .25f)]
        float _minDistanceThreshold = .05f;

        CharacterController2D _characterController;
        Vector2 _targetPosition;

        void Awake()
        {
            _characterController = GetComponent<CharacterController2D>();
        }

        void Update()
        {
            if (Vector2.Distance(transform.position, _targetPosition) > _minDistanceThreshold)
                _characterController.SetInputs(transform.position.x > _targetPosition.x ? -1 : 1, false);
            else
            {
                MoveCompleted?.Invoke();
                enabled = false;
            }
        }

        public void MoveToPosition(Vector2 position)
        {
            enabled = true;
            _targetPosition = position;
        }
    }
}