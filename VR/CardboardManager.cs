using UnityEngine;

namespace Demonixis.Toolbox.VR
{
    public class CardboardManager : VRDeviceManager
    {
        private Cardboard cardboard = null;

        #region Public Fields

        public override bool IsEnabled
        {
            get { return cardboard != null ? cardboard.VRModeEnabled : false; }
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
            get { return Cardboard.SDK.StereoScreenScale; }
            set { Cardboard.SDK.StereoScreenScale = value; }
        }

        public override VRDeviceType VRDeviceType
        {
            get { return VRDeviceType.Cardboard; }
        }

        #endregion

        public override void SetVREnabled(bool isEnabled)
        {
            if (!IsPresent)
                return;

            if (cardboard == null)
            {
                var camera = Camera.main.gameObject;
                var parent = camera.transform.parent.gameObject;

                camera.AddComponent<StereoController>();
                parent.AddComponent<CardboardHead>();
                cardboard = gameObject.AddComponent<Cardboard>();
            }

            cardboard.VRModeEnabled = isEnabled;
        }

        public override void Recenter()
        {
#if UNITY_ANDROID
            Cardboard.SDK.Recenter();
#endif
        }
    }
}