using System.Collections;
using UnityEngine;

namespace Recall.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    [DisallowMultipleComponent]
    public class SplashFader : MonoBehaviour
    {
        [SerializeField, Range(.5f, 3)]
        float _fadeTime = 2;
        [SerializeField, Range(.1f, 3)]
        float _showTime = 2;
        [SerializeField]
        AnimationCurve _fadeCurve;
        [SerializeField]
        CanvasGroup _logoGroup;

        CanvasGroup _mainGroup;

        void Awake()
        {
            _mainGroup = GetComponent<CanvasGroup>();
        }

        void OnEnable()
        {
            _mainGroup.alpha = 1;
            FadeSplash();
        }

        Coroutine FadeSplash()
        {
            return StartCoroutine(splashFadeRoutine());
            IEnumerator splashFadeRoutine()
            {
                _mainGroup.alpha = 1;
                yield return FadeToAlpha(1, _logoGroup);
                yield return new WaitForSeconds(_showTime);
                _mainGroup.blocksRaycasts = false;
                yield return FadeToAlpha(0, _mainGroup);
                gameObject.SetActive(false);
            }
        }

        IEnumerator FadeToAlpha(float value, CanvasGroup group)
        {
            float source = group.alpha;
            float target = value;

            var endOfFrame = new WaitForEndOfFrame();
            float time = 0;
            while (time < _fadeTime)
            {
                time += Time.deltaTime;
                group.alpha = Mathf.Lerp(source, target, _fadeCurve.Evaluate(time / _fadeTime));
                yield return endOfFrame;
            }
        }
    }
}