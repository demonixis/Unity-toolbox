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
        private bool _isFading = false;
        private short _sign = 1;
        private ScreenOverlay _overlay;
        private float fadeSpeed = 0.5f;

        private Canvas _canvas = null;
        private bool _canvasWasSwitched = false;

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
        [SerializeField]
        private string _mainCanvasTag = "MainUI";

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
            if (_isFading)
            {
                _overlay.intensity += fadeSpeed * (Time.deltaTime > 0 ? Time.deltaTime : Mathf.Clamp(Time.unscaledDeltaTime, 0, 0.2f)) * _sign;

                if ((_sign == 1 && _overlay.intensity >= 1) || (_sign == -1 && _overlay.intensity <= 0))
                {
                    _overlay.intensity = _sign == 1 ? 1 : 0;
                    _isFading = false;

                    if (_overlay.intensity <= 0)
                        _overlay.enabled = false;

                    if (FadeCompleted != null)
                        FadeCompleted();

                    if (_sign == -1)
                        SwitchCanvasMode(false);
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

                var canvasGo = GameObject.FindWithTag(_mainCanvasTag);
                if (canvasGo != null)
                    _canvas = canvasGo.GetComponent<Canvas>();
            }
        }

        /// <summary>
        /// Switches the current main canvas to screenspace camera only if it uses the ScreenSpaceOverlay mode.
        /// </summary>
        /// <param name="switchToScreenSpace"></param>
        private void SwitchCanvasMode(bool switchToScreenSpace)
        {
            if (UnityEngine.VR.VRSettings.enabled || _canvas == null)
                return;

            if (switchToScreenSpace && _canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                _canvas.renderMode = RenderMode.ScreenSpaceCamera;
                _canvas.worldCamera = Camera.main;
                _canvasWasSwitched = true;
            }
            else if (_canvasWasSwitched)
            {
                _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                _canvas.worldCamera = null;
                _canvasWasSwitched = false;
            }
        }

        public void StartFade(short sign)
        {
            _sign = sign;
            _overlay.intensity = _sign == 1 ? 0 : 1;
            _overlay.enabled = true;
            _isFading = true;
            SwitchCanvasMode(true);
        }

        public void StopFade()
        {
            _isFading = false;
            _overlay.enabled = false;
            _overlay.intensity = _sign == 1 ? 0 : 1;
            SwitchCanvasMode(false);
        }

        public static void FadeIn(float fadeSpeed = 2.5f, Action fadeCompleted = null)
        {
            Fade(-1, fadeSpeed, fadeCompleted);
        }

        public static void FadeOut(float fadeSpeed = 2.5f, Action fadeCompleted = null)
        {
            Fade(1, fadeSpeed, fadeCompleted);
        }

        public static void FadeOutIn(float fadeSpeed = 2.5f, Action onFadeOut = null, Action onFadeIn = null)
        {
            FadeOut(fadeSpeed, () =>
            {
                if (onFadeOut != null)
                    onFadeOut();

                FadeIn(fadeSpeed, onFadeIn);
            });
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

        public static bool IsFading()
        {
            if (s_faders == null)
                s_faders = FindObjectsOfType<ScreenFader>();

            if (s_faders.Length == 0)
                return false;

            return s_faders[0]._isFading;
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
