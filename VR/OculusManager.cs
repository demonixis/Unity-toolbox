using System.Collections;
using UnityEngine;
using UnityEngine.VR;

namespace Demonixis.Toolbox.VR
{
    /// <summary>
    /// OculusManager - Manages all aspect of the VR from this singleton.
    /// </summary>
    public sealed class OculusManager : VRManager
    {
        private static string[] AnchorNames = new string[]
        {
            "LeftEyeAnchor",
            "RightEyeAnchor",
            "TrackerAnchor",
            "LeftHandAnchor",
            "RightHandAnchor"
        };

        private static OculusManager _instance = null;
        private OVRManager ovrManager = null;

        [SerializeField]
        private bool queueAhead = true;
        [SerializeField]
        private OVRManager.TrackingOrigin trackingOriginType = OVRManager.TrackingOrigin.EyeLevel;
        [SerializeField]
        private bool usePositionTracking = true;
        [SerializeField]
        private bool resetTrackerOnLoad = false;

        #region Singleton

        public static OculusManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<OculusManager>();

                    if (_instance == null)
                    {
                        var go = new GameObject("OculusManager");
                        _instance = go.AddComponent<OculusManager>();
                    }
                }

                return _instance;
            }
        }

        #endregion

        protected override void CheckInstance()
        {
            if (_instance != null && _instance != this)
                Destroy(this);
            else if (_instance == null)
                _instance = this;
        }

        public IEnumerator RecenterView()
        {
            yield return new WaitForEndOfFrame();
            Recenter();
        }

        public override void SetVREnabled(bool isEnabled)
        {
            if (!VRDevice.isPresent || VRSettings.loadedDevice != VRDeviceType.Oculus)
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
                ovrManager.queueAhead = _instance.queueAhead;
                ovrManager.trackingOriginType = _instance.trackingOriginType;
                ovrManager.usePositionTracking = _instance.usePositionTracking;
                ovrManager.resetTrackerOnLoad = _instance.resetTrackerOnLoad;
            }

			if (isEnabled)
				StartCoroutine(RecenterView());

            vrEnabled = isEnabled;
            VRSettings.enabled = isEnabled;
        }

        private void CreateDefaultStructure(Transform trackingSpace)
        {
            for (int i = 0, l = AnchorNames.Length; i < l; i++)
                CreateAnchor(trackingSpace, AnchorNames[i]);
        }

        private void CreateAnchor(Transform trackingSpace, string name)
        {
            if (!trackingSpace.Find(name))
            {
                var anchor = new GameObject(name);
                anchor.transform.parent = trackingSpace;
            }
        }

        #region Static Methods

        public static Vector3 GetLocalPosition(byte viewerIndex)
        {
            return InputTracking.GetLocalPosition((VRNode)viewerIndex);
        }

        public static Quaternion GetLocalRotation(uint viewerIndex)
        {
            return InputTracking.GetLocalRotation((VRNode)viewerIndex);
        }

        public static void Recenter()
        {
            InputTracking.Recenter();
        }

        public static void SetRenderScale(float scale)
        {
            VRSettings.renderScale = scale;
        }

        public static void SetGPULevel(byte value)
        {
#if UNITY_ANDROID
            OVRPlugin.gpuLevel = value;
#endif
        }

        public static void SetCPULevel(byte value)
        {
#if UNITY_ANDROID
            OVRPlugin.cpuLevel = value;
#endif
        }

        #endregion
    }
}