using System;
using UnityEngine;

namespace Demonixis.Toolbox.VR
{
    public class GoogleVRManager : VRDeviceManager
    {
        private GvrViewer gvrViewer = null;

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
            get
            {
#if UNITY_ANDROID
                // Don't load if if the GearVR mode is enabled.
                // Must be changed when Unity will supports DayDream
                return !UnityEngine.VR.VRDevice.isPresent && IsSupported;
#else
                return false;
#endif
            }
        }

        public override float RenderScale
        {
            get { return GvrViewer.Instance.StereoScreenScale; }
            set { GvrViewer.Instance.StereoScreenScale = value; }
        }

        public override VRDeviceType VRDeviceType
        {
            get { return VRDeviceType.GoogleVR; }
        }

        public override Vector3 HeadPosition
        {
            get { return Vector3.zero; }
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
                var parent = camera.transform.parent.gameObject;

                camera.AddComponent<StereoController>();
                parent.AddComponent<GvrHead>();
                gvrViewer = gameObject.AddComponent<GvrViewer>();
            }

            gvrViewer.VRModeEnabled = isEnabled;
        }

        public override void Recenter()
        {
#if UNITY_ANDROID
            GvrViewer.Instance.Recenter();
#endif
        }
    }
}