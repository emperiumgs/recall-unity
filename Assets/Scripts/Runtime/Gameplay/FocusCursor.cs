using UnityEngine;

namespace Recall.Gameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public class FocusCursor : MonoBehaviour
    {
        public enum CursorState : byte
        {
            Free,
            Hovering,
            Selecting
        }

        CursorState _state;
        Animator _animator;
        int _stateHash;

        void Awake()
        {
            _animator = GetComponent<Animator>();

            _stateHash = Animator.StringToHash("FocusState");
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void SetState(CursorState state)
        {
            _state = state;
            _animator.SetInteger(_stateHash, (int)_state);
        }
    }
}