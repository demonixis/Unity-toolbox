#define USE_CARDBOARD_SDK_
#define USE_OSVR_SDK

using UnityEngine.VR;

namespace Demonixis.Toolbox.VR
{
    public static class GameVRSettings
    {
        public enum VRDeviceType
        {
            None = 0, Cardboard, UnityNative, OSVR
        }

        private static bool _osvrChecked = false;
        private static VRDeviceType _deviceType = VRDeviceType.None;
        public static bool ForceNativeVR = false;
        public static bool ForceOSVR = false;
        public static bool ForceCardboardVR = false;


        public static VRDeviceType GetVRDevice()
        {
#if UNITY_STANDALONE
            HasOSVRHMDEnabled();
#endif
#if UNITY_ANDROID || UNITY_EDITOR
            //HasGoogleCardboardEnabled();
#endif
#if UNITY_ANDROID || UNITY_STANDALONE || UNITY_EDITOR
            HasUnityVRHMDEnabled();
#endif
            return _deviceType;
        }

        public static bool VRModeEnabled()
        {
            return GetVRDevice() != VRDeviceType.None;
        }

        public static bool HasUnityVRHMDEnabled()
        {
            if (VRDevice.isPresent && VRDevice.family != "none")
                _deviceType = VRDeviceType.UnityNative;

            if (ForceNativeVR)
                _deviceType = VRDeviceType.UnityNative;

            return _deviceType == VRDeviceType.UnityNative;
        }

        public static bool HasGoogleCardboardEnabled()
        {
            if ((HasGoogleCardboardSupport() && !HasUnityVRHMDEnabled()) || ForceCardboardVR)
                _deviceType = VRDeviceType.Cardboard;

            return _deviceType == VRDeviceType.Cardboard;
        }

        public static bool HasGoogleCardboardSupport()
        {
#if UNITY_EDITOR
            return true;
#else
        return UnityEngine.SystemInfo.supportsGyroscope;
#endif
        }

        public static bool HasOSVRHMDEnabled(bool forceCheck = false)
        {
#if UNITY_STANDALONE
            if (!_osvrChecked || forceCheck)
            {
                if (_deviceType == VRDeviceType.OSVR)
                    _deviceType = VRDeviceType.None;

                var clientKit = OSVR.Unity.ClientKit.instance;
                if (clientKit != null)
                {
                    var clientContext = clientKit.context;
                    if (clientContext != null && clientContext.CheckStatus())
                        _deviceType = VRDeviceType.OSVR;
                }

                _osvrChecked = true;
            }
#endif
            if (ForceOSVR)
                _deviceType = VRDeviceType.OSVR;

            return _deviceType == VRDeviceType.OSVR;
        }

        public static float GetRenderScale()
        {
            if (_deviceType == VRDeviceType.UnityNative)
                return VRSettings.renderScale;
#if USE_CARDBOARD_SDK
            else if (_deviceType == VRDeviceType.Cardboard)
                Cardboard.SDK.StereoScreenScale = scale;
#endif

            return 1.0f;
        }

        public static void SetRenderScale(float scale)
        {
            if (_deviceType == VRDeviceType.UnityNative)
                VRSettings.renderScale = scale;
#if USE_CARDBOARD_SDK
            else if (_deviceType == VRDeviceType.Cardboard)
                Cardboard.SDK.StereoScreenScale = scale;
#endif
        }

        public static void Recenter()
        {
            if (_deviceType == GameVRSettings.VRDeviceType.UnityNative)
                InputTracking.Recenter();
#if USE_OSVR_SDK
            else if (_deviceType == GameVRSettings.VRDeviceType.OSVR)
                OsvrManager.Recenter();
#endif
#if USE_CARDBOARD_SDK
            else if (_deviceType == GameVRSettings.VRDeviceType.Cardboard)
                Cardboard.SDK.Recenter();
#endif
        }
    }
}