using Cinemachine;
using UnityEngine;

namespace Recall.Gameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController2D))]
    public class CharacterFlip : MonoBehaviour
    {
        [SerializeField]
        SpriteRenderer[] _renderers;
        [SerializeField]
        Transform[] _transforms;
        [SerializeField]
        CinemachineVirtualCamera _virtualCamera;

        CharacterController2D _characterController;

        void Awake()
        {
            _characterController = GetComponent<CharacterController2D>();
            _characterController.FlippedStateChanged += OnFlip;
        }

        void OnFlip(bool isFlipped)
        {
            foreach (var renderer in _renderers)
                renderer.flipX = isFlipped;

            int multiplier = isFlipped ? -1 : 1;
            foreach (var transform in _transforms)
                transform.localPosition = new Vector3(Mathf.Abs(transform.localPosition.x) * multiplier, transform.localPosition.y);

            if (_virtualCamera)
            {
                var transposer = _virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
                transposer.m_TrackedObjectOffset.x = Mathf.Abs(transposer.m_TrackedObjectOffset.x) * multiplier;
            }
        }
    }
}