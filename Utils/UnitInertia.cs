using UnityEngine;

namespace MarsExtraction.Utils
{
    [ExecuteInEditMode]
    public sealed class UnitInertia : MonoBehaviour
    {
        private Transform _transform;
        private Vector3 _position = Vector3.zero;
        private float _y = 0;
        private float _originalY = 0;
        private bool _enabled = true;

        public float min = 1.5f;
        public float max = 0.5f;
        public float frequency = 0.1f;
        public float phase = 0.0f;

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;

                if (!_enabled)
                {
                    _position = _transform.localPosition;
                    _position.y = _originalY;
                    _transform.localPosition = _position;
                }
            }
        }

        void Awake()
        {
            _transform = GetComponent<Transform>();
            _originalY = _transform.localPosition.y;

            if (phase == 0.0f)
                phase = Random.Range(0.0f, 8000.0f);
        }

        void Update()
        {
            if (_enabled)
            {
                _position = _transform.localPosition;

                _y = (Time.time + phase) * frequency;
                _y = _y - Mathf.Floor(_y); // normalized value to 0..1
                _position.y = ((max * Mathf.Sin(2 * Mathf.PI * _y)) + min);

                _transform.localPosition = _position;
            }
        }
    }
}