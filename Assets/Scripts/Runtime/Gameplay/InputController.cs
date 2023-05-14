using UnityEngine;

namespace Recall.Gameplay
{
    [DisallowMultipleComponent]
    public class InputController : MonoBehaviour
    {
        CharacterController2D _characterController;
        CharacterAnimator _characterAnimator;

        void Awake()
        {
            _characterController = GetComponent<CharacterController2D>();
            _characterAnimator = GetComponent<CharacterAnimator>();
        }

        void Update()
        {
            var horizontal = Input.GetAxis("Horizontal");
            _characterController.SetInputs(horizontal, Input.GetButtonDown("Jump"));
            _characterAnimator.SetHorizontalInput(horizontal);
        }
    }
}