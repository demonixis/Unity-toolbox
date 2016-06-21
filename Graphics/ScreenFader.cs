using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace Demonixis.Toolbox.Graphics
{
    [RequireComponent(typeof(ScreenOverlay))]
    [RequireComponent(typeof(Camera))]
    public sealed class ScreenFader : MonoBehaviour
    {
        public enum FadeMode
        {
            FadeIn = 0, FadeOut
        }

        private static ScreenFader[] s_faders;
        private bool _enabled = false;
        private short _sign = 1;
        private ScreenOverlay _overlay;
        private float fadeSpeed = 0.5f;

        ///
        /// Gets or sets the overlay intensity.
        ///
        public float Intensity
        {
            get { return _overlay.intensity; }
            set { _overlay.intensity = value; }
        }

        public Action FadeCompleted = null;

        [SerializeField]
        private bool _fadeOnStart = true;

        #region GameState Pattern

        void OnEnable()
        {
            Initialize();

            if (_fadeOnStart)
                StartFade(-1);
        }

        void OnDestroy()
        {
            if (s_faders != null)
                s_faders = null;
        }

        void Update()
        {
            if (_enabled)
            {
                _overlay.intensity += fadeSpeed * (Time.deltaTime > 0 ? Time.deltaTime : Time.unscaledDeltaTime) * _sign;

                if ((_sign == 1 && _overlay.intensity >= 1) || (_sign == -1 && _overlay.intensity <= 0))
                {
                    _overlay.intensity = _sign == 1 ? 1 : 0;
                    _enabled = false;

                    if (_overlay.intensity <= 0)
                        _overlay.enabled = false;

                    if (FadeCompleted != null)
                        FadeCompleted();
                }
            }
        }

        #endregion

        public void Initialize()
        {
            if (_overlay == null)
            {
                _overlay = GetComponent<ScreenOverlay>();
                if (_overlay == null)
                    _overlay = (ScreenOverlay)gameObject.AddComponent(typeof(ScreenOverlay));

                _overlay.blendMode = ScreenOverlay.OverlayBlendMode.AlphaBlend;
                _overlay.intensity = 0.0f;

                if (_overlay.texture == null)
                {
                    _overlay.texture = new Texture2D(1, 1);
                    _overlay.texture.SetPixel(0, 0, Color.black);
                    _overlay.texture.Apply();
                }

                _overlay.enabled = false;
            }
        }

        public void StartFade(short sign)
        {
            _sign = sign;
            _overlay.intensity = _sign == 1 ? 0 : 1;
            _overlay.enabled = true;
            _enabled = true;
        }

        public void StopFade()
        {
            _enabled = false;
            _overlay.enabled = false;
            _overlay.intensity = _sign == 1 ? 0 : 1;
        }

        public static void FadeIn(float fadeSpeed = 2.5f, Action fadeCompleted = null)
        {
            Fade(-1, fadeSpeed, fadeCompleted);
        }

        public static void FadeOut(float fadeSpeed = 2.5f, Action fadeCompleted = null)
        {
            Fade(1, fadeSpeed, fadeCompleted);
        }

        public static void Reset(bool fill)
        {
            if (s_faders == null)
                s_faders = FindObjectsOfType<ScreenFader>();

            if (s_faders.Length == 0)
                return;

            for (int i = 0, l = s_faders.Length; i < l; i++)
                s_faders[i].Intensity = fill ? 1.0f : 0.0f;
        }

        public static Texture2D GetOverlayTexture()
        {
            if (s_faders == null)
                s_faders = FindObjectsOfType<ScreenFader>();

            if (s_faders == null || s_faders.Length == 0)
                return null;

            return s_faders[0]._overlay.texture;
        }

        public static void SetOverlayTexture(Texture2D texture)
        {
            if (s_faders == null)
                s_faders = FindObjectsOfType<ScreenFader>();

            if (s_faders == null || s_faders.Length == 0)
                return;

            for (int i = 0, l = s_faders.Length; i < l; i++)
                s_faders[i]._overlay.texture = texture;
        }

        private static void Fade(short sign, float fadeSpeed = 2.5f, Action fadeCompleted = null)
        {
            if (s_faders == null)
                s_faders = FindObjectsOfType<ScreenFader>();

            if (s_faders.Length == 0)
            {
                if (fadeCompleted != null)
                    fadeCompleted();

                return;
            }

            for (int i = 0, l = s_faders.Length; i < l; i++)
            {
                s_faders[i].Initialize();
                s_faders[i].fadeSpeed = fadeSpeed;

                if (i == 0)
                    s_faders[i].FadeCompleted = fadeCompleted;

                s_faders[i].StartFade(sign);
            }
        }
    }
}
