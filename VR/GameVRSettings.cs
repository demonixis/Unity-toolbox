using System;
using UnityEngine;
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
        public static bool ForceNativeVR = false;
        public static bool ForceOSVR = false;
        public static bool ForceCardboardVR = false;
        public static VRDeviceType DeviceType { get; set; }

        public static VRDeviceType GetVRDevice()
        {
#if UNITY_STANDALONE
            HasOSVRHMDEnabled();
#endif
            HasGoogleCardboardEnabled();
            HasUnityVRHMDEnabled();
            return DeviceType;
        }

        public static bool VRModeEnabled()
        {
            return GetVRDevice() != VRDeviceType.None;
        }

        public static bool HasUnityVRHMDEnabled()
        {
#if UNITY_STANDALONE || UNITY_ANDROID
            if (VRDevice.isPresent && VRDevice.family != "none")
                DeviceType = VRDeviceType.UnityNative;
#endif
            if (ForceNativeVR)
                DeviceType = VRDeviceType.UnityNative;

            return DeviceType == VRDeviceType.UnityNative;
        }

        public static bool HasGoogleCardboardEnabled()
        {
            if ((HasGoogleCardboardSupport() && !HasUnityVRHMDEnabled()) || ForceCardboardVR)
                DeviceType = VRDeviceType.Cardboard;

            return DeviceType == VRDeviceType.Cardboard;
        }

        public static bool HasGoogleCardboardSupport()
        {
#if UNITY_ANDROID || UNITY_IOS
#if UNITY_EDITOR
        return true;
#else
        return UnityEngine.SystemInfo.supportsGyroscope;
#endif
#else
            return false;
#endif
        }

        public static bool HasOSVRHMDEnabled(bool forceCheck = false)
        {
#if UNITY_STANDALONE
            if (!_osvrChecked || forceCheck)
            {
                if (DeviceType == VRDeviceType.OSVR)
                    DeviceType = VRDeviceType.None;

                try
                {
                    var clientKit = OSVR.Unity.ClientKit.instance;
                    if (clientKit != null)
                    {
                        var clientContext = clientKit.context;
                        if (clientContext != null && clientContext.CheckStatus())
                            DeviceType = VRDeviceType.OSVR;
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log("[VR] OSVR Not available");
                    Debug.Log(ex.Message);
                }

                _osvrChecked = true;
            }
#endif
            if (ForceOSVR)
                DeviceType = VRDeviceType.OSVR;

            return DeviceType == VRDeviceType.OSVR;
        }

        public static void Recenter()
        {
            if (DeviceType == GameVRSettings.VRDeviceType.OSVR)
                Demonixis.Toolbox.VR.OSVRRecenter.Recenter();
            else if (DeviceType == GameVRSettings.VRDeviceType.UnityNative)
                InputTracking.Recenter();
        }
    }
}