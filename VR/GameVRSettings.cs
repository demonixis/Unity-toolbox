#define USE_CARDBOARD_SDK
#define USE_OSVR_SDK

using UnityEngine.VR;

namespace Demonixis.Toolbox.VR
{
    public static class GameVRSettings
    {
        public enum VRDeviceType
        {
            None = 0,
            Cardboard,
            UnityVR,
            OSVR
        }

        private static VRDeviceType deviceType = VRDeviceType.None;
        private static bool cardboardEnabled = false;

        public static bool UnityVREnabled
        {
            get { return VRDevice.isPresent && VRSettings.enabled; }
            set
            {
                if (VRDevice.isPresent)
                    VRSettings.enabled = value;
            }
        }

        public static bool CardboardEnabled
        {
            get { return cardboardEnabled; }
            set
            {
                if (CardboardSupported)
                    cardboardEnabled = value;
            }
        }

        public static bool CardboardSupported
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

        public static bool OSVREnabled
        {
            get
            {
#if USE_OSVR_SDK
                var clientKit = OSVR.Unity.ClientKit.instance;
                if (clientKit != null)
                {
                    var clientContext = clientKit.context;
                    if (clientContext != null && clientContext.CheckStatus())
                        return true;
                }
#endif

                return false;
            }
        }

        public static bool VREnabled
        {
            get { return CheckVRDevices() != VRDeviceType.None; }
        }

        public static float RenderScale
        {
            get
            {
                if (deviceType == VRDeviceType.UnityVR)
                    return VRSettings.renderScale;
#if USE_CARDBOARD_SDK
                else if (deviceType == VRDeviceType.Cardboard)
                    return Cardboard.SDK.StereoScreenScale;
#endif

                return 1.0f;
            }
            set
            {
                if (deviceType == VRDeviceType.UnityVR)
                    VRSettings.renderScale = value;
#if USE_CARDBOARD_SDK
                else if (deviceType == VRDeviceType.Cardboard)
                    Cardboard.SDK.StereoScreenScale = value;
#endif
            }
        }

        public static VRDeviceType CheckVRDevices()
        {
            if (deviceType == VRDeviceType.None)
            {
                if (UnityVREnabled)
                    deviceType = VRDeviceType.UnityVR;
#if USE_CARDBOARD_SDK
                else if (CardboardEnabled)
                    deviceType = VRDeviceType.Cardboard;
#endif
#if USE_OSVR_SDK
                else if (OSVREnabled)
                    deviceType = VRDeviceType.OSVR;
#endif
            }

            return deviceType;
        }

        public static void Recenter()
        {
            if (deviceType == GameVRSettings.VRDeviceType.UnityVR)
                InputTracking.Recenter();
#if USE_CARDBOARD_SDK
            else if (deviceType == GameVRSettings.VRDeviceType.Cardboard)
                Cardboard.SDK.Recenter();
#endif
#if USE_OSVR_SDK
            else if (deviceType == GameVRSettings.VRDeviceType.OSVR)
                OsvrManager.Recenter();
#endif
        }
    }
}