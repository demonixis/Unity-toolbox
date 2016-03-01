using UnityEngine;

namespace Demonixis.VR
{
    public class UVRDistortionCorrectionEffect : MonoBehaviour
    {
        private Shader _shader;
        private Material _material;
        private Vector4 _hmdWarpParam;
        private Vector2 _scale;
        private Vector2 _scaleIn;
        private Vector2 _lensCenter;
        private float _aspect = 1.0f;
        private float[] _kernel = new float[4] { 1.48f, 0.22f, 0.24f, 0.0f };
        private float _scaleFactor = 1.86f;
        private float _lensCenterOffset = 0;

        public bool isLeftEye = false;

        public bool NeedsUpdate { get; set; }

        private Material material
        {
            get
            {
                if (_material == null)
                {
                    _material = new Material(_shader);
                    _material.hideFlags = HideFlags.HideAndDontSave;
                }

                return _material;
            }
        }

        void Start()
        {
            if (!SystemInfo.supportsImageEffects)
                enabled = false;

            _shader = Resources.Load("Shaders/UVR_DistortionCorrection") as Shader;

            if (_shader == null || !_shader.isSupported)
                enabled = false;

            UpdateValues();
        }

        void Update()
        {
            if (NeedsUpdate)
            {
                UpdateValues();
                NeedsUpdate = false;
            }
        }

        void OnDisable()
        {
            if (_material)
                DestroyImmediate(_material);
        }

        void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
        {
            if (_shader != null)
            {
                UpdateValues();
                material.SetVector("_HmdWarpParam", _hmdWarpParam);
                material.SetVector("_Scale", _scale);
                material.SetVector("_ScaleIn", _scaleIn);
                material.SetVector("_LensCenter", _lensCenter);
                Graphics.Blit(sourceTexture, destTexture, _material);
            }
            else
                Graphics.Blit(sourceTexture, destTexture);
        }

        private void UpdateValues()
        {
            _aspect = (Screen.width * 0.5f) / Screen.height;
            _scale = new Vector2(0.5f * (1.0f / _scaleFactor), 0.5f * (1.0f / _scaleFactor) * _aspect);
            _scaleIn = new Vector2(2.0f, 2.0f / _aspect);
            _lensCenter = isLeftEye ? new Vector2(0.5f + _lensCenterOffset * 0.5f, 0.5f) : new Vector2(0.5f - _lensCenterOffset * 0.5f, 0.5f);
            _hmdWarpParam = new Vector4(_kernel[0], _kernel[1], _kernel[2], _kernel[3]);
        }

        public void Setup(float[] kernel, float scaleFactor, float lensCenter)
        {
            _kernel = kernel;
            _scaleFactor = scaleFactor;
            _lensCenterOffset = lensCenter;
            NeedsUpdate = true;
        }
    }
}
