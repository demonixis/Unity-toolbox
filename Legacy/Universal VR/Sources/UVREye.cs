using UnityEngine;

namespace Demonixis.VR
{
    public class UVREye : MonoBehaviour
    {
        private Camera _camera;
        private Matrix4x4 _projection;
        private UVRDistortionCorrectionEffect _effect;
        private float _aspect = 1.0f;
        private bool _needsUpdate = false;
        private float _zoom = 0.1f;
        private float _IPDCorrection = 0.0f;
        private float _near = 0.1f;
        private float _far = 10000.0f;

        public bool isLeftEye = false;

        public bool DistortionCorrection
        {
            get { return _effect.enabled; }
            set
            {
                if (_effect.enabled != value)
                {
                    _effect.enabled = value;

                    if (value)
                        _effect.NeedsUpdate = true;
                }
            }
        }

        void Awake()
        {
            _effect = GetComponent<UVRDistortionCorrectionEffect>();

            if (_effect == null)
                _effect = gameObject.AddComponent<UVRDistortionCorrectionEffect>();

            _effect.isLeftEye = isLeftEye;
            _effect.NeedsUpdate = true;
        }

        void Start()
        {
            var sdk = GetComponentInParent(typeof(UVRManager)) as UVRManager;
            _effect.enabled = sdk != null ? sdk.DistortionCorrection : false;
            _camera = GetComponent(typeof(Camera)) as Camera;
            SetCorrection(_IPDCorrection);
        }

        void Update()
        {
            if (_needsUpdate)
            {
                SetCorrection(_IPDCorrection);
                _needsUpdate = false;
            }
        }

        public void Setup(float zoom, float ipdCorrection, float near, float far)
        {
            _zoom = zoom;
            _IPDCorrection = ipdCorrection;
            _near = near;
            _far = far;
        }

        private void SetCorrection(float correction)
        {
            var left = (isLeftEye ? -_zoom + correction : -_zoom - correction) * (_near / 0.1f);
            var right = (isLeftEye ? _zoom + correction : _zoom - correction) * (_near / 0.1f);

            CreatePerspectiveOffCenter(left, right, -_zoom * (_near / 0.1f) * _aspect, _zoom * (_near / 0.1f) * _aspect, _near, _far, ref _projection);

            _camera.projectionMatrix = _projection;
        }

        private static void CreatePerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far, ref Matrix4x4 result)
        {
            result[0, 0] = 2.0f * near / (right - left);
            result[0, 1] = 0;
            result[0, 2] = (right + left) / (right - left);
            result[0, 3] = 0;
            result[1, 0] = 0;
            result[1, 1] = 2.0f * near / (top - bottom);
            result[1, 2] = (top + bottom) / (top - bottom);
            result[1, 3] = 0;
            result[2, 0] = 0;
            result[2, 1] = 0;
            result[2, 2] = -(far + near) / (far - near);
            result[2, 3] = -(2.0f * far * near) / (far - near);
            result[3, 0] = 0;
            result[3, 1] = 0;
            result[3, 2] = -1.0f;
            result[3, 3] = 0;
        }
    }
}