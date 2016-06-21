using UnityEngine;

namespace Demonixis.Toolbox.Controllers
{
    /// <summary>
    /// SimpleMouseRotator: Easily turn an object on X/Y axis.
    /// It's a fork of the Unity's version of SimpleMouseRotator.
    /// </summary>
    public abstract class SimpleAbstractRotator : MonoBehaviour
    {
        // Caches
        private Vector3 _targetAngles = Vector3.zero;
        private Vector3 _followAngles = Vector3.zero;
        private Vector3 _followVelocity = Vector3.zero;
        private Quaternion _originalRotation = Quaternion.identity;
        private Transform _transform = null;
        private float _inputHorizontal = 0.0f;
        private float _inputVertical = 0.0f;

        [Header("Move Settings")]
        [SerializeField]
        public Vector2 _rotationRange = new Vector3(90.0f, 960.0f);
        [SerializeField]
        public float _rotationSpeed = 5.0f;
        [SerializeField]
        public float _damping = 0.1f;
        [SerializeField]
        protected float _sensibilityX = 1.0f;
        [SerializeField]
        protected float _sensibilityY = 1.0f;

        protected virtual void Start()
        {
            _transform = (Transform)GetComponent(typeof(Transform));
            _originalRotation = _transform.localRotation;
        }

        protected virtual void Update()
        {
            UpdateInput(ref _inputHorizontal, ref _inputVertical);
            UpdateControls();
        }

        protected abstract void UpdateInput(ref float horizontal, ref float vertical);

        private void UpdateControls()
        {
            // we make initial calculations from the original local rotation
            _transform.localRotation = _originalRotation;

            // wrap values to avoid springing quickly the wrong way from positive to negative
            if (_targetAngles.y > 180)
            {
                _targetAngles.y -= 360;
                _followAngles.y -= 360;
            }

            if (_targetAngles.x > 180)
            {
                _targetAngles.x -= 360;
                _followAngles.x -= 360;
            }

            if (_targetAngles.y < -180)
            {
                _targetAngles.y += 360;
                _followAngles.y += 360;
            }

            if (_targetAngles.x < -180)
            {
                _targetAngles.x += 360;
                _followAngles.x += 360;
            }

            _targetAngles.y += _inputHorizontal * _rotationSpeed;
            _targetAngles.x += _inputVertical * _rotationSpeed;

            // clamp values to allowed range
            _targetAngles.y = Mathf.Clamp(_targetAngles.y, -_rotationRange.y * 0.5f, _rotationRange.y * 0.5f);
            _targetAngles.x = Mathf.Clamp(_targetAngles.x, -_rotationRange.x * 0.5f, _rotationRange.x * 0.5f);

            // smoothly interpolate current values to target angles
            _followAngles = Vector3.SmoothDamp(_followAngles, _targetAngles, ref _followVelocity, _damping);

            // update the actual gameobject's rotation
            _transform.localRotation = _originalRotation * Quaternion.Euler(-_followAngles.x, _followAngles.y, 0);
        }
    }
}