using UnityEngine;

namespace Recall.Gameplay
{
    public class PuzzleBehavior : MonoBehaviour
    {
        [SerializeField]
        PressurePlateBehavior[] _pressurePlates;
        [SerializeField]
        DoorBehavior _sectorDoor;

        void Awake()
        {
            foreach (var plate in _pressurePlates)
                plate.StateChanged += ValidatePlates;
        }

        void ValidatePlates(bool value)
        {
            foreach (var plate in _pressurePlates)
                if (!plate.IsPressed)
                    return;
            
            _sectorDoor.OpenAndUnlock();
        }
    }
}