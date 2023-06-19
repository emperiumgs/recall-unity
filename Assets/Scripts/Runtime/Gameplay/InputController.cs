using UnityEngine;

namespace Recall.Gameplay
{
    [DisallowMultipleComponent]
    public class InputController : MonoBehaviour
    {
        CharacterController2D _characterController;
        CharacterAnimator _characterAnimator;
        CharacterCombat _characterCombat;

        void Awake()
        {
            _characterController = GetComponent<CharacterController2D>();
            _characterAnimator = GetComponent<CharacterAnimator>();
            _characterCombat = GetComponent<CharacterCombat>();
        }

        void Update()
        {
            if (Input.GetButtonDown("Shield"))
                _characterCombat.SetDefending(true);
            else if (Input.GetButtonUp("Shield"))
                _characterCombat.SetDefending(false);

            if (Input.GetButtonDown("Fire1"))
                _characterCombat.Attack();

            var horizontal = Input.GetAxis("Horizontal");
            _characterController.SetInputs(horizontal, Input.GetButtonDown("Jump"));
            _characterAnimator.SetHorizontalInput(horizontal);
        }
    }
}