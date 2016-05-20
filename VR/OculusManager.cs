using System;
using UnityEngine;
using UnityEngine.VR;

namespace Demonixis.Toolbox.VR
{
    /// <summary>
    /// OculusManager - Manages all aspect of the VR from this singleton.
    /// </summary>
    public sealed class OculusManager : VRDeviceManager
    {
        private OVRManager ovrManager = null;

        #region Inspector Fields

        [SerializeField]
        private bool _queueAhead = true;
        [SerializeField]
        private OVRManager.TrackingOrigin _trackingOriginType = OVRManager.TrackingOrigin.EyeLevel;
        [SerializeField]
        private bool _usePositionTracking = true;
        [SerializeField]
        private bool _resetTrackerOnLoad = false;

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
            get { return VRSettings.enabled && OVRManager.isHmdPresent; }
        }

        public override bool IsPresent
        {
            get { return  VRDevice.isPresent && OVRManager.isHmdPresent; }
        }

        public override float RenderScale
        {
            get { return VRSettings.renderScale; }
            set { VRSettings.renderScale = value; }
        }

        public override VRDeviceType VRDeviceType
        {
            get { return VRDeviceType.UnityVR; }
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

#if UNITY_ANDROID
            if (QualitySettings.vSyncCount != 0)
                QualitySettings.vSyncCount = 0;
#endif
            
            if (isEnabled && ovrManager == null)
            {
                var camera = Camera.main.GetComponent<Transform>();
                var trackingSpace = camera.parent;

                if (trackingSpace == null || trackingSpace.parent == null)
                    throw new UnityException("[OculusManager] Your prefab doesn't respect the correct hierarchy");

                CreateDefaultStructure(trackingSpace);

                var head = trackingSpace.parent.gameObject;
                head.AddComponent<OVRCameraRig>();

                ovrManager = head.AddComponent<OVRManager>();
                ovrManager.queueAhead = _queueAhead;
                ovrManager.trackingOriginType = _trackingOriginType;
                ovrManager.usePositionTracking = _usePositionTracking;
                ovrManager.resetTrackerOnLoad = _resetTrackerOnLoad;
            }
            
            if (isEnabled)
                Recenter();

            VRSettings.enabled = isEnabled;
        }

        public override void Recenter()
        {
            InputTracking.Recenter();
        }

        private void CreateDefaultStructure(Transform trackingSpace)
        {
            var anchorNames = new string[]
            {
                "LeftEyeAnchor",
                "RightEyeAnchor",
                "TrackerAnchor",
                "LeftHandAnchor",
                "RightHandAnchor"
            };

            for (int i = 0, l = anchorNames.Length; i < l; i++)
                CreateAnchor(trackingSpace, anchorNames[i]);
        }

        private void CreateAnchor(Transform trackingSpace, string name)
        {
            if (!trackingSpace.Find(name))
            {
                var anchor = new GameObject(name);
                anchor.transform.parent = trackingSpace;
            }
        }
    }
}