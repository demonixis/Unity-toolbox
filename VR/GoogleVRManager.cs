/// GameVRSettings
/// Last Modified Date: 08/16/2016

using System;
using System.Collections;
using UnityEngine;

namespace Demonixis.Toolbox.VR
{
    public class GoogleVRManager : VRDeviceManager
    {
        private GvrViewer gvrViewer = null;

        #region Editor Fields

        [Tooltip("Add the name of the components you want to copy on the created cameras. Usefull for post processes.")]
        [SerializeField]
        private string[] _cameraComponentsToKeep = null;
        [Tooltip("Add the name of the components you want to remove from the main camera. Usefull for unused post processes.")]
        [SerializeField]
        private string[] _cameraComponentsToRemove = null;

        #endregion

        #region Public Fields

        public override bool IsEnabled
        {
            get { return gvrViewer != null ? gvrViewer.VRModeEnabled : false; }
        }

        public static bool IsSupported
        {
            get
            {
#if UNITY_EDITOR
                return true;
#else
                return UnityEngine.SystemInfo.supportsGyroscope;
#endif
            }
        }

        public override bool IsAvailable
        {
            get { return Detect; }
        }

        public static bool Detect
        {
            get
            {
#if UNITY_EDITOR
                return true;
#else
                return !UnityEngine.VR.VRDevice.isPresent && UnityEngine.SystemInfo.supportsGyroscope;
#endif
            }
        }

        public override float RenderScale
        {
            get { return gvrViewer != null ? gvrViewer.StereoScreenScale : 1.0f; }
            set
            {
                if (gvrViewer != null)
                    gvrViewer.StereoScreenScale = value;
            }
        }

        public override VRDeviceType VRDeviceType
        {
            get { return VRDeviceType.GoogleVR; }
        }

        public override Vector3 HeadPosition
        {
            get { return gvrViewer != null ? gvrViewer.HeadPose.Position : Vector3.zero; }
        }

        #endregion

        public override void Dispose()
        {
            Destroy(this);
        }

        public override void SetVREnabled(bool isEnabled)
        {
            if (!IsAvailable)
                return;

            if (gvrViewer == null)
            {
                var camera = Camera.main.gameObject;
                gvrViewer = camera.AddComponent<GvrViewer>();

                StartCoroutine(MoveUsefullCameraComponents(camera));
            }

            gvrViewer.VRModeEnabled = isEnabled;
        }

        public override void Recenter()
        {
#if UNITY_ANDROID
            if (gvrViewer != null)
                gvrViewer.Recenter();
#endif
        }

        private IEnumerator MoveUsefullCameraComponents(GameObject camera)
        {
            if (_cameraComponentsToKeep != null)
            {
                yield return new WaitForSeconds(0.5f);

                var postRender = camera.GetComponentInChildren<GvrPostRender>();

                for (int i = 0, l = _cameraComponentsToKeep.Length; i < l; i++)
                    CopyComponent(camera.GetComponent(_cameraComponentsToKeep[i]), postRender.gameObject);
            }

            for (int i = 0, l = _cameraComponentsToRemove.Length; i < l; i++)
                Destroy(GetComponent(_cameraComponentsToRemove[i]));

            yield return null;
        }
    }
}