using UnityEngine;

namespace Recall.Gameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public class CharacterAnimator : MonoBehaviour
    {
        CharacterController2D _characterController;
        Animator _animator;
        int _walkingHash;
        int _jumpHash;

        void Awake()
        {
            _characterController = GetComponent<CharacterController2D>();
            _characterController.GroundedStateChanged += OnGroundedStateChanged;
            _characterController.Jumped += OnJump;

            _animator = GetComponent<Animator>();

            _walkingHash = Animator.StringToHash("walking");
            _jumpHash = Animator.StringToHash("jump");
        }

        public void SetHorizontalInput(float input)
        {
            _animator.SetBool(_walkingHash, input != 0);
        }

        void OnJump()
        {
            _animator.SetBool(_jumpHash, true);
        }

        void OnGroundedStateChanged(bool isGrounded)
        {
            _animator.SetBool(_jumpHash, !isGrounded);
        }
    }
}