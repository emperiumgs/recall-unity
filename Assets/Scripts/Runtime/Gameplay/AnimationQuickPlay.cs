using UnityEngine;

namespace Recall.Gameplay
{
    [RequireComponent(typeof(Animation))]
    public class AnimationQuickPlay : MonoBehaviour
    {
        Animation _animation;

        void Awake()
        {
            _animation = GetComponent<Animation>();
        }

        public void Play() => _animation.Play();
    }
}