#if UNITY_ANDROID
using System;
using System.Collections.Generic;
#endif
using System.Collections;
using UnityEngine;

namespace Demonixis.Toolbox.VR
{
    public class GoogleVRDevice : VRDeviceBase
    {
#if UNITY_ANDROID && false
        private GvrViewer gvrViewer = null;
#endif
        [SerializeField]
        private Vector3 _headPosition = new Vector3(0.0f, 1.8f, 0.0f);
        [SerializeField]
        private float _maxResolution = 0;
        [SerializeField]
        private Transform _cameraNode = null;
        [SerializeField]
        private string[] _cameraComponentsToMove = null;

#if UNITY_EDITOR
        [SerializeField]
        private bool _DisableInEditor = false;
#endif

        #region Public Fields

#if UNITY_ANDROID && false
        public GvrViewer GvrViewer
        {
            get { return gvrViewer; }
        }
#endif

        public override bool IsAvailable
        {
            get
            {
#if UNITY_ANDROID && false
#if UNITY_EDITOR
                return !UnityEngine.VR.VRDevice.isPresent && !_DisableInEditor;
#else
                // Don't load if if the GearVR mode is enabled.
                // Must be changed when Unity will supports DayDream
                return !UnityEngine.VR.VRDevice.isPresent && SystemInfo.supportsGyroscope;
#endif
#else
                return false;
#endif
            }
        }

        public override float RenderScale
        {
#if UNITY_ANDROID && false
            get { return GvrViewer.Instance.StereoScreenScale; }
            set { GvrViewer.Instance.StereoScreenScale = value; }
#else
            get; set;
#endif
        }

        public override VRDeviceType VRDeviceType
        {
            get { return VRDeviceType.GoogleVR; }
        }

        public override Vector3 HeadPosition
        {
            get { return _headPosition; }
        }

        public override int EyeTextureWidth
        {
#if UNITY_ANDROID && false
            get
            {
                if (gvrViewer != null)
                    return (int)(Screen.width * gvrViewer.StereoScreenScale);
                else
                    return Screen.width;
            }
#else
            get { return Screen.height; }
#endif
        }

        public override int EyeTextureHeight
        {
#if UNITY_ANDROID && false
            get
            {
                if (gvrViewer != null)
                    return (int)(Screen.height * gvrViewer.StereoScreenScale);
                else
                    return Screen.height;
            }
#else
            get { return Screen.width; }
#endif
        }

        #endregion

        public override void SetActive(bool isEnabled)
        {
            if (!IsAvailable)
                return;

#if UNITY_ANDROID && false
            if (gvrViewer == null)
            {
                GvrViewer.Create();
                gvrViewer = GvrViewer.Instance;

                if (_maxResolution > 0)
                {
                    var screenWidth = (float)Screen.currentResolution.width;
                    var scale = screenWidth > _maxResolution ? _maxResolution / screenWidth : 1;
                    gvrViewer.StereoScreenScale = scale;
                }

                if (_cameraNode != null && _cameraComponentsToMove != null && _cameraComponentsToMove.Length > 0)
                    StartCoroutine(CopyComponentsToEyes());
            }

            gvrViewer.VRModeEnabled = isEnabled;
#endif
        }

        private IEnumerator CopyComponentsToEyes()
        {
            yield return new WaitForEndOfFrame();
#if UNITY_ANDROID && false
            var component = (Component)null;
            var postRender = FindObjectOfType<GvrPostRender>();
            var componentsToDestroy = new List<Component>();

            for (var i = 0; i < _cameraComponentsToMove.Length; i++)
            {
                component = _cameraNode.GetComponent(_cameraComponentsToMove[i]);

                if (component != null && CopyComponent(component, postRender.gameObject) != null)
                    componentsToDestroy.Add(component);
            }

            for (var i = 0; i < componentsToDestroy.Count; i++)
                Destroy(componentsToDestroy[i]);
#endif
        }

        public override void Dispose()
        {
            Destroy(this);
        }

        public override void Recenter()
        {
#if UNITY_ANDROID && false
            if (gvrViewer != null)
                gvrViewer.Recenter();
#endif
        }
    }
}