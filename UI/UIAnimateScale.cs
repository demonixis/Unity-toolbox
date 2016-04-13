using UnityEngine;

namespace Demonixis.Toolbox.UI
{
    public sealed class UIAnimateScale : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private bool _done = false;
        public Vector3 startScale = new Vector3(0.2f, 0.2f, 0.2f);
        public float scaleSpeed = 2.0f;
        public bool ignoreTimeScale = true;

        void OnEnable()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
                return;
#endif
            Reset();
        }

        void Update()
        {
            if (!_done)
            {
                _rectTransform.localScale = Vector3.Lerp(_rectTransform.localScale, Vector3.one, (ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime) * scaleSpeed);

                if (Vector3.Distance(_rectTransform.localScale, Vector3.one) < 0.1f)
                {
                    _done = true;
                    _rectTransform.localScale = Vector3.one;
                }
            }
        }

        public void Reset()
        {
            if (_rectTransform == null)
                _rectTransform = (RectTransform)GetComponent(typeof(RectTransform));

            _rectTransform.localScale = startScale;
            _done = false;
        }
    }
}