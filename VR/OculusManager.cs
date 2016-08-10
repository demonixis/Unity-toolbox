/// GameVRSettings
/// Last Modified Date: 08/10/2016

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VR;

namespace Demonixis.Toolbox.VR
{
    /// <summary>
    /// OculusManager - Manages all aspect of the VR from this singleton.
    /// </summary>
    public sealed class OculusManager : UnityVRDevice
    {
        private static string UnityVR_Name = "Oculus";
        private OVRManager ovrManager = null;

        #region Inspector Fields

        [Header("Oculus SDK Settings")]
        [SerializeField]
        private bool _queueAhead = true;
        [SerializeField]
        private OVRManager.TrackingOrigin _trackingOriginType = OVRManager.TrackingOrigin.EyeLevel;
        [SerializeField]
        private bool _usePositionTracking = true;
        [SerializeField]
        private bool _resetTrackerOnLoad = false;
        [SerializeField]
        private bool _addControllersNode = false;
        [Tooltip("You can attach the OVRTrackerBounds to a disabled GameObject. It'll be parented to the main camera.")]
        [SerializeField]
        private Transform _trackerBounds = null;
        [Tooltip("Or you can use a prefab, but you've to update this field on each scene.")]
        [SerializeField]
        private GameObject _trackerBoundsPrefab = null;

        #endregion

        #region Public Fields

        /// <summary>
        /// Gets or sets the CPU level (Android only)
        /// </summary>
        public int CPULevel
        {
            get { return OVRPlugin.cpuLevel; }
            set { OVRPlugin.cpuLevel = value; }
        }

        /// <summary>
        /// Gets or sets the GPU level (Android only)
        /// </summary>
        public int GPULevel
        {
            get { return OVRPlugin.gpuLevel; }
            set { OVRPlugin.gpuLevel = value; }
        }

        public override bool IsEnabled
        {
            get { return Detect; }
        }

        public override bool IsAvailable
        {
            get { return VRDevice.isPresent && VRSettings.loadedDeviceName == UnityVR_Name; }
        }

        public override string UnityVRName
        {
            get { return UnityVR_Name; }
        }

        public static bool Detect
        {
            get { return VRSettings.enabled && VRSettings.loadedDeviceName == UnityVR_Name; }
        }

        #endregion

        public override void SetVREnabled(bool isEnabled)
        {
#if UNITY_ANDROID
            if (QualitySettings.vSyncCount != 0)
                QualitySettings.vSyncCount = 0;
#endif

            if (isEnabled && ovrManager == null)
            {
                var camera = Camera.main.GetComponent<Transform>();
                camera.name = "CenterEyeAnchor";

                if (camera.parent == null || camera.parent.parent == null)
                    throw new UnityException("[OculusManager] Your prefab doesn't respect the correct hierarchy");

                var trackingSpace = camera.parent;
                trackingSpace.name = "TrackingSpace";

                var head = trackingSpace.parent.gameObject;
                head.name = "OVRCameraRig";
                head.AddComponent<OVRCameraRig>();

                // We store the head transform and its initial position for future calibrations.
                m_headTransform = head.GetComponent<Transform>();
                m_originalHeadPosition = m_headTransform.localPosition;

                ovrManager = head.AddComponent<OVRManager>();
                ovrManager.queueAhead = _queueAhead;
                ovrManager.trackingOriginType = _trackingOriginType;
                ovrManager.usePositionTracking = _usePositionTracking;
                ovrManager.resetTrackerOnLoad = _resetTrackerOnLoad;

                if (_trackerBoundsPrefab != null)
                {
                    var trackerBoundsGO = Instantiate<GameObject>(_trackerBoundsPrefab);
                    trackerBoundsGO.name = "TrackerBounds";
                    var trackerBoundsTransform = trackerBoundsGO.GetComponent<Transform>();
                    trackerBoundsTransform.parent = camera;
                    trackerBoundsTransform.localPosition = Vector3.zero;
                    trackerBoundsTransform.localRotation = Quaternion.identity;
                }
                else if (_trackerBounds != null)
                {
                    _trackerBounds.parent = camera;
                    _trackerBounds.localPosition = Vector3.zero;
                    _trackerBounds.localRotation = Quaternion.identity;
                }

                if (_fixHeadPosition)
                    StartCoroutine(ResetHeadPosition(0.5f));

                CreateDefaultStructure(head.transform.parent, trackingSpace);
            }

            VRSettings.enabled = isEnabled;
        }

        private void CreateDefaultStructure(Transform player, Transform trackingSpace)
        {
            CreateNode(player, "ForwardDirection");

            if (_addControllersNode)
                CreateNode(player, "OVR_Controllers");

            var anchorNames = new string[]
            {
                "LeftEyeAnchor",
                "RightEyeAnchor",
                "TrackerAnchor",
                "LeftHandAnchor",
                "RightHandAnchor"
            };

            for (int i = 0, l = anchorNames.Length; i < l; i++)
                CreateNode(trackingSpace, anchorNames[i]);
        }

        private GameObject CreateNode(Transform parent, string name)
        {
            GameObject node = null;
            Transform nodeTransform = null;

            if (!parent.Find(name))
            {
                node = new GameObject(name);
                nodeTransform = node.GetComponent<Transform>();
                nodeTransform.parent = parent;
                nodeTransform.localPosition = Vector3.zero;
                nodeTransform.localRotation = Quaternion.identity;
            }

            return node;
        }
    }
}