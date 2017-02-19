/// GameVRSettings
/// Last Modified Date: 08/10/2016

using UnityEngine;
#if UNITY_STANDALONE_WIN || UNITY_ANDROID
using UnityEngine.VR;
#endif

namespace Demonixis.Toolbox.VR
{
    /// <summary>
    /// OculusManager - Manages all aspect of the VR from this singleton.
    /// </summary>
    public sealed class OculusDevice : UnityVRDevice
    {
        private const string UnityVR_Name = "Oculus";

#if UNITY_STANDALONE_WIN || UNITY_ANDROID
        private OVRManager ovrManager = null;
#endif
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
#if UNITY_STANDALONE_WIN || UNITY_ANDROID
            get { return OVRPlugin.cpuLevel; }
            set { OVRPlugin.cpuLevel = value; }
#else
            get; set;
#endif
        }

        /// <summary>
        /// Gets or sets the GPU level (Android only)
        /// </summary>
        public int GPULevel
        {
#if UNITY_STANDALONE_WIN || UNITY_ANDROID
            get { return OVRPlugin.gpuLevel; }
            set { OVRPlugin.gpuLevel = value; }
#else
            get; set;
#endif
        }

        public override bool IsAvailable
        {
#if UNITY_STANDALONE_WIN || UNITY_ANDROID
            get { return VRDevice.isPresent && VRSettings.loadedDeviceName == UnityVR_Name; }
#else
            get { return false; }
#endif
        }

        public static bool Detect
        {
#if UNITY_STANDALONE_WIN || UNITY_ANDROID
            get { return VRSettings.enabled && VRSettings.loadedDeviceName == UnityVR_Name; }
#else
            get { return false; }
#endif
        }

#endregion

        public override void SetActive(bool active)
        {
#if UNITY_ANDROID
            if (QualitySettings.vSyncCount != 0)
                QualitySettings.vSyncCount = 0;
#endif

#if UNITY_STANDALONE_WIN || UNITY_ANDROID
            if (active && ovrManager == null)
            {
                var camera = Camera.main.GetComponent<Transform>();
                camera.name = "CenterEyeAnchor";

                if (camera.parent == null || camera.parent.parent == null)
                    throw new UnityException("[OculusManager] Your prefab doesn't respect the correct hierarchy");

                var trackingSpace = camera.parent;
                trackingSpace.name = "TrackingSpace";

                var head = trackingSpace.parent.gameObject;
                head.name = "OVRCameraRig";

                if (head.GetComponent<OVRCameraRig>() == null)
                {
                    head.AddComponent<OVRCameraRig>();

                    // We store the head transform and its initial position for future calibrations.
                    m_headTransform = head.GetComponent<Transform>();

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

                    CreateDefaultStructure(head.transform.parent, trackingSpace);
                }
            }

            VRSettings.enabled = active;
#endif
        }

        private void CreateDefaultStructure(Transform player, Transform trackingSpace)
        {
            CreateNode(player, "ForwardDirection");

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