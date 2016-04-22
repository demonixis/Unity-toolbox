﻿#define USE_INCONTROL
#if USE_INCONTROL
using InControl;
#endif
using UnityEngine;

namespace Demonixis.Toolbox.Controllers
{
    /// <summary>
    /// SimpleMouseRotator: Easily turn an object on X/Y axis.
    /// It's a fork of the Unity's version of SimpleMouseRotator.
    /// </summary>
    public sealed class SimpleMouseRotator : MonoBehaviour
    {
        // Caches
        private Vector3 _targetAngles;
        private Vector3 _followAngles;
        private Vector3 _followVelocity;
        private Quaternion _originalRotation;
        private Transform _transform;
        private float _inputH = 0.0f;
        private float _inputV = 0.0f;

        [SerializeField]
        private float _sensibilityX = 0.25f;
        [SerializeField]
        private float _sensibilityY = 0.25f;
        [SerializeField]
        public Vector2 _rotationRange = new Vector3(180, 70);
        [SerializeField]
        public float _rotationSpeed = 10;
        [SerializeField]
        public float _damping = 0.2f;

        void Start()
        {
            _transform = (Transform)GetComponent(typeof(Transform));
            _originalRotation = _transform.localRotation;
        }

        void Update()
        {
#if USE_INCONTROL
            var device = InputManager.ActiveDevice;
            _inputH = device.RightStickX * _sensibilityX;
            _inputV = device.RightStickY * _sensibilityY;
#else
            _inputH = Input.GetAxis("Mouse X") * _sensibilityX;
            _inputV = Input.GetAxis("Mouse Y") * _sensibilityY;
#endif

            UpdateControls();
        }

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

            _targetAngles.y += _inputH * _rotationSpeed;
            _targetAngles.x += _inputV * _rotationSpeed;

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