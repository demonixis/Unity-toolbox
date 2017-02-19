/// GameVRSettings
/// Last Modified Date: 08/10/2016

using System;
using System.Collections;
using UnityEngine;

namespace Demonixis.Toolbox.VR
{
    /// <summary>
    /// Defines the type of SDK.
    /// </summary>
    public enum VRDeviceType
    {
        None = 0,
        UnityVR,
        OSVR,
        GoogleVR
    }

    /// <summary>
    /// The GameVRSettings is responsible to check available VR devices and select the one with the higher priority.
    /// It's also used to Recenter the view.
    /// </summary>
    public sealed class VRManager : MonoBehaviour
    {
        #region Private Fields

        private static VRDeviceBase activeVRDevice = null;
        private bool _vrChecked = false;

        #endregion

        [SerializeField]
        private bool _fixHeadPosition = false;
        [SerializeField]
        private Transform _headNode = null;
        [SerializeField]
        private Vector3 _headFixAxis = Vector3.up;

        #region Instance Methods

        private void Awake()
        {
            StartCoroutine(CheckVRDevices());
        }

        private void OnDestroy()
        {
            activeVRDevice = null;
        }

        private IEnumerator CheckVRDevices()
        {
            var endOfFrame = new WaitForEndOfFrame();
            var camera = Camera.main;

            while (camera == null)
            {
                camera = Camera.main;
                yield return endOfFrame;
            }

            GetVRDevice();
        }

        /// <summary>
        /// Gets the type of VR device currently connected. It takes the first VR device which have the higher priority.
        /// </summary>
        /// <returns></returns>
        public VRDeviceType GetVRDevice()
        {
            if (_vrChecked)
                return activeVRDevice != null ? activeVRDevice.VRDeviceType : VRDeviceType.None;

            // Gets all managers and enable only the first connected device.
            var vrManagers = GetComponents<VRDeviceBase>();
            var count = vrManagers.Length;
            var deviceType = VRDeviceType.None;

            activeVRDevice = null;

            if (count > 0)
            {
                Array.Sort(vrManagers);

                for (var i = 0; i < count; i++)
                {
                    if (vrManagers[i].IsAvailable && deviceType == VRDeviceType.None)
                    {
                        activeVRDevice = vrManagers[i];
                        activeVRDevice.SetActive(true);
                        deviceType = activeVRDevice.VRDeviceType;
                        ResetOrientationAndPosition();
                    }
                    else
                        vrManagers[i].Dispose();
                }
            }

            _vrChecked = true;

            return deviceType;
        }

        public void DestroyAll()
        {
            var devices = GetComponents<VRDeviceBase>();
            for (var i = 0; i < devices.Length; i++)
                Destroy(devices[i]);

            Destroy(this);
        }

        /// <summary>
        /// Recenter the head position relative to the head position
        /// </summary>
        /// <param name="time">Time to wait before reseting</param>
        /// <returns></returns>
        public void ResetOrientationAndPosition()
        {
            Recenter();

            if (ActiveDevice == null || !_fixHeadPosition)
                return;

            if (_headNode == null)
                _headNode = GetComponent<Transform>();
            
            var targetPosition = ActiveDevice.HeadPosition;

            if (targetPosition == Vector3.zero)
                return;

            if (_headFixAxis.x < 1)
                targetPosition.x = 0.0f;

            if (_headFixAxis.y < 1)
                targetPosition.y = 0.0f;

            if (_headFixAxis.z < 1)
                targetPosition.z = 0.0f;

            _headNode.localPosition = targetPosition;
        }

        #endregion

        #region Static Fields

        /// <summary>
        /// Recenter the view of the active manager.
        /// </summary>
        public static void Recenter()
        {
            if (activeVRDevice != null)
                activeVRDevice.Recenter();
        }

        /// <summary>
        /// Gets or sets the render scale.
        /// </summary>
        public static float RenderScale
        {
            get
            {
                if (activeVRDevice == null)
                    return 1.0f;

                return activeVRDevice.RenderScale;
            }
            set
            {
                if (activeVRDevice != null)
                    activeVRDevice.RenderScale = value;
            }
        }

        public static int EyeTextureWidth
        {
            get
            {
                if (activeVRDevice == null)
                    return Screen.width;

                return activeVRDevice.EyeTextureWidth;
            }
        }

        public static int EyeTextureHeight
        {
            get
            {
                if (activeVRDevice == null)
                    return Screen.height;

                return activeVRDevice.EyeTextureHeight;
            }
        }

        /// <summary>
        /// Indicates if the VR mode is enabled.
        /// </summary>
        public static bool Enabled
        {
            get { return activeVRDevice != null; }
        }


        /// <summary>
        /// Gets the current active VR device.
        /// </summary>
        public static VRDeviceBase ActiveDevice
        {
            get { return activeVRDevice; }
        }


        #endregion
    }
}