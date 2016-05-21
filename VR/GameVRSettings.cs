#if UNITY_ANDROID
#define USE_CARDBOARD_SDK
#endif
#if UNITY_STANDALONE || UNITY_ANDROID
#define USE_OSVR_SDK
#define USE_OVR_SDK
#endif
#if UNITY_STANDALONE
#define USE_OPENVR_SDK
#endif

using UnityEngine;
using UnityEngine.VR;

namespace Demonixis.Toolbox.VR
{
    public enum VRDeviceType
    {
        None = 0,
        Cardboard,
        UnityVR,
        OSVR
    }

    public sealed class GameVRSettings : MonoBehaviour
    {
        private static VRDeviceManager activeManager = null;
        private bool _vrChecked = false;

        [SerializeField]
        private KeyCode _recenterKey = KeyCode.None;

        void Awake()
        {
            GetVRDevice();
        }

        void Update()
        {
            if (Input.GetKeyDown(_recenterKey))
                Recenter();
        }

        /// <summary>
        /// Gets the type of VR device currently connected. It takes the first VR device which have the higher priority.
        /// </summary>
        /// <param name="forceCheck">Set to true to force the check.</param>
        /// <returns></returns>
        public VRDeviceType GetVRDevice(bool forceCheck = false)
        {
            if (_vrChecked && !forceCheck)
                return activeManager != null ? activeManager.VRDeviceType : VRDeviceType.None;

            // Gets all managers and enable only the first connected device.
            var vrManagers = GetComponents<VRDeviceManager>();
            var count = vrManagers.Length;
            var deviceType = VRDeviceType.None;

            activeManager = null;

            if (count > 0)
            {
                System.Array.Sort<VRDeviceManager>(vrManagers);

                for (var i = 0; i < count; i++)
                {
                    if (vrManagers[i].IsPresent && deviceType == VRDeviceType.None)
                    {
                        activeManager = vrManagers[i];
                        activeManager.SetVREnabled(true);
                        deviceType = activeManager.VRDeviceType;
                        continue;
                    }

                    vrManagers[i].Dispose();
                }
            }

            return deviceType;
        }

        #region Static Fields

        /// <summary>
        /// Gets the active VR Device Manager.
        /// </summary>
        public static VRDeviceManager ActiveManager
        {
            get { return activeManager; }
        }

        /// <summary>
        /// Indicates if Cardboard is enabled.
        /// </summary>
        public static bool CardboardEnabled
        {
            get
            {
#if USE_CARDBOARD_SDK
                var cardboard = Cardboard.SDK;
                if (cardboard != null && cardboard.VRModeEnabled)
                    return true;
#endif
                return false;
            }
        }

        /// <summary>
        /// Indicates if Oculus Rift/GearVR is enabled.
        /// </summary>
        public static bool OculusEnabled
        {
            get
            {
#if USE_OVR_SDK
                return VRSettings.enabled && OVRManager.isHmdPresent;
#else
                return false;
#endif
            }
        }

        /// <summary>
        /// Indicates if OSVR is enabled.
        /// </summary>
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

        /// <summary>
        /// Indicates if OpenVR is enabled.
        /// </summary>
        public static bool OpenVREnabled
        {
            get
            {
#if USE_OPENVR_SDK
                return VRSettings.enabled && Valve.VR.OpenVR.IsHmdPresent();
#else

                return false;
#endif
            }
        }

        /// <summary>
        /// Recenter the view of the active manager.
        /// </summary>
        public static void Recenter()
        {
            if (activeManager != null)
                activeManager.Recenter();
        }

        /// <summary>
        /// Gets or sets the render scale.
        /// </summary>
        public static float RenderScale
        {
            get
            {
                if (activeManager == null)
                    return 1.0f;

                return activeManager.RenderScale;
            }
            set
            {
                if (activeManager != null)
                    activeManager.RenderScale = value;
            }
        }

        /// <summary>
        /// Indicates if UnityEngine.VR is enabled.
        /// If a device is connected, it'll return true for PSVR, Oculus and OpenVR.
        /// </summary>
        public static bool UnityVREnabled
        {
            get { return VRDevice.isPresent && VRSettings.enabled; }
            set
            {
                if (VRDevice.isPresent)
                    VRSettings.enabled = value;
            }
        }

        /// <summary>
        /// Indicates if the VR mode is enabled.
        /// </summary>
        public static bool VREnabled
        {
            get { return activeManager != null; }
        }

        /// <summary>
        /// Gets the type of VR device.
        /// </summary>
        public static VRDeviceType VRDeviceType
        {
            get { return activeManager != null ? activeManager.VRDeviceType : VRDeviceType.None; }
        }

        #endregion
    }
}