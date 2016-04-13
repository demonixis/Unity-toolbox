using UnityEngine;
using UnityEngine.UI;

namespace Demonixis.Toolbox.UI
{
    [RequireComponent(typeof(Image))]
    public sealed class UIFadePanel : MonoBehaviour
    {
        public enum InterpolationMode { Normal = 0, Lerp, SmoothStep }
        private Image _background;
        private Color _transparentColor;
        private Color _targetColor;
        private Color _originalColor;
        private bool _done = true;
        public float fadeSpeed = 2.0f;
        public bool ignoreTimeScale = true;
        public InterpolationMode Mode = InterpolationMode.Lerp;

        public void Reset()
        {
            EnsureInitialized();
            InternalReset();
        }

        void Update()
        {
            if (!_done)
            {
                if (Mode == InterpolationMode.Lerp)
                    _targetColor.a = Mathf.Lerp(_targetColor.a, _originalColor.a, (ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime) * fadeSpeed);
                else if (Mode == InterpolationMode.SmoothStep)
                    _targetColor.a = Mathf.SmoothStep(_targetColor.a, _originalColor.a, (ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime) * fadeSpeed);
                else
                    _targetColor.a += (ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime) * fadeSpeed;

                _background.color = _targetColor;

                if (_targetColor.a >= _originalColor.a)
                    _done = true;
            }
        }

        private void InternalReset()
        {
            _targetColor = _originalColor;
            _targetColor.a = 0.0f;
            _background.color = _transparentColor;
            _done = _background != null ? false : true;
        }

        private void EnsureInitialized()
        {
            if (_background == null)
            {
                _background = GetComponent<Image>();
                _originalColor = _background.color;
                _transparentColor = _originalColor;
                _transparentColor.a = 0.0f;
            }
        }
    }
}