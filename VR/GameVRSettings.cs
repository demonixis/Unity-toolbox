/// GameVRSettings
/// Last Modified Date: 08/10/2016

#if UNITY_ANDROID
#define GOOGLE_VR_SDK_
#define OCULUS_SDK
#endif
#if UNITY_STANDALONE
#define OSVR_SDK_
#define OCULUS_SDK
#define OPENVR_SDK
#endif

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VR;

namespace Demonixis.Toolbox.VR
{
    /// <summary>
    /// Defines the type of SDK.
    /// </summary>
    public enum VRDeviceType
    {
        None = 0,
        GoogleVR,
        UnityVR,
        OSVR
    }

    /// <summary>
    /// The GameVRSettings is responsible to check available VR devices and select the one with the higher priority.
    /// It's also used to Recenter the view.
    /// </summary>
    public sealed class GameVRSettings : MonoBehaviour
    {
        #region Private Fields

        private static VRDeviceManager s_ActiveVRDevice = null;
        private bool _vrChecked = false;

        #endregion

        #region Editor Fields

        [Header("Default Settings")]
        [SerializeField]
        private KeyCode _recenterKey = KeyCode.None;
        [SerializeField]
        private string _recenterButton = string.Empty;

        #endregion

        #region Instance Methods

        void Awake()
        {
            if (enabled)
                GetVRDevice();
        }

        void Update()
        {
            if (Input.GetKeyDown(_recenterKey) || (_recenterButton != string.Empty && Input.GetButtonDown(_recenterButton)))
                Recenter();
        }

        /// <summary>
        /// Gets the type of VR device currently connected. It takes the first VR device which have the higher priority.
        /// </summary>
        /// <param name="forceCheck">Set to true to force the check.</param>
        /// <returns></returns>
        public VRDeviceType GetVRDevice()
        {
            if (_vrChecked)
                return s_ActiveVRDevice != null ? s_ActiveVRDevice.VRDeviceType : VRDeviceType.None;

            // Gets all managers and enable only the first connected device.
            var vrManagers = GetComponents<VRDeviceManager>();
            var count = vrManagers.Length;
            var deviceType = VRDeviceType.None;

            s_ActiveVRDevice = null;

            if (count > 0)
            {
                Array.Sort(vrManagers);

                for (var i = 0; i < count; i++)
                {
                    if (vrManagers[i].IsAvailable && deviceType == VRDeviceType.None)
                    {
                        s_ActiveVRDevice = vrManagers[i];
                        s_ActiveVRDevice.SetVREnabled(true);
                        deviceType = s_ActiveVRDevice.VRDeviceType;

                        StartCoroutine(RecenterEndOfFrame());

                        continue;
                    }

                    vrManagers[i].Dispose();
                }
            }

            return deviceType;
        }

        /// <summary>
        /// Recenter the view at the very end of the life cycle process.
        /// </summary>
        /// <returns></returns>
        private IEnumerator RecenterEndOfFrame()
        {
            yield return new WaitForEndOfFrame();

            if (s_ActiveVRDevice != null)
                s_ActiveVRDevice.Recenter();
        }

        #endregion

        #region Static Fields

        /// <summary>
        /// Gets the active VR Device Manager.
        /// </summary>
        public static VRDeviceManager ActiveVRDevice
        {
            get { return s_ActiveVRDevice; }
        }

        /// <summary>
        /// Indicates if Cardboard is enabled.
        /// </summary>
        public static bool GoogleVREnabled
        {
            get
            {
#if UNITY_ANDROID
                if (VRSettings.enabled && VRSettings.loadedDeviceName == "Oculus")
                    return false;
#if GOOGLE_VR_SDK
                return ActiveVRDevice != null && ActiveVRDevice is GoogleVRManager;
#endif
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
#if OCULUS_SDK
                return VRSettings.enabled && VRSettings.loadedDeviceName == "Oculus";
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
#if OSVR_SDK
                return OSVRManager.Detect;
#else

                return false;
#endif
            }
        }

        /// <summary>
        /// Indicates if OpenVR is enabled.
        /// </summary>
        public static bool OpenVREnabled
        {
            get
            {
#if OPENVR_SDK
                return VRSettings.enabled && VRSettings.loadedDeviceName == "OpenVR";
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
            if (s_ActiveVRDevice != null)
                s_ActiveVRDevice.Recenter();
        }

        /// <summary>
        /// Gets or sets the render scale.
        /// </summary>
        public static float RenderScale
        {
            get
            {
                if (s_ActiveVRDevice == null)
                    return 1.0f;

                return s_ActiveVRDevice.RenderScale;
            }
            set
            {
                if (s_ActiveVRDevice != null)
                    s_ActiveVRDevice.RenderScale = value;
            }
        }

        /// <summary>
        /// Indicates if UnityEngine.VR is enabled.
        /// If a device is connected, it'll return true for PSVR, OCULUS_SDK and OpenVR.
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
            get { return s_ActiveVRDevice != null; }
        }

        /// <summary>
        /// Gets the type of VR device.
        /// </summary>
        public static VRDeviceType VRDeviceType
        {
            get { return s_ActiveVRDevice != null ? s_ActiveVRDevice.VRDeviceType : VRDeviceType.None; }
        }

#endregion
    }
}