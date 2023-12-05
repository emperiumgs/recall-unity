using Cinemachine;
using UnityEngine;

namespace Recall.Gameplay
{
    public class CameraTargetBinder : MonoBehaviour
    {
        CinemachineVirtualCamera _virtualCamera;

        void OnEnable()
        {
            _virtualCamera = GetComponent<CinemachineVirtualCamera>();

            var character = FindAnyObjectByType<FocusBehavior>();
            if (character)
                _virtualCamera.Follow = character.transform;
        }
    }
}