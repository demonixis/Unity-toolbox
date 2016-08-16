/// GameVRSettings
/// Last Modified Date: 08/16/2016

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
                gvrViewer = Camera.main.gameObject.AddComponent<GvrViewer>();

            gvrViewer.VRModeEnabled = isEnabled;
        }

        public override void Recenter()
        {
#if UNITY_ANDROID
            if (gvrViewer != null)
                gvrViewer.Recenter();
#endif
        }
    }
}