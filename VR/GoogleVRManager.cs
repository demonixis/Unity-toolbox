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

        public override bool IsPresent
        {
            get
            {
#if UNITY_ANDROID
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

        #endregion

        public override void Dispose()
        {
            Destroy(this);
        }

        public override void SetVREnabled(bool isEnabled)
        {
            if (!IsPresent)
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