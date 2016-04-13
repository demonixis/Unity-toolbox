using System.Collections;
using UnityEngine;
using UnityEngine.VR;

namespace Demonixis.Toolbox.VR
{
    /// <summary>
    /// OculusManager - Manages all aspect of the VR from this singleton.
    /// </summary>
    public sealed class OculusManager : MonoBehaviour
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
        private bool m_vrEnabled = true;
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

        #region Default Unity pattern

        void Awake()
        {
            CheckInstance();
        }

        void OnEnabled()
        {
            CheckInstance();
        }

        void Start()
        {
            if (m_vrEnabled)
                SetVREnabled(true);
        }

        private void CheckInstance()
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

        #endregion

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

        public static void SetVREnabled(bool vrEnabled)
        {
            if (!VRDevice.isPresent || VRDevice.model != "Oculus")
                return;

#if UNITY_ANDROID
            if (QualitySettings.vSyncCount != 0)
                QualitySettings.vSyncCount = 0;
#endif
            var ovrManager = Instance.ovrManager;

            if (vrEnabled && ovrManager == null)
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
                Instance.ovrManager = ovrManager;
            }

            VRSettings.enabled = vrEnabled;
        }

        private static void CreateDefaultStructure(Transform trackingSpace)
        {
            for (int i = 0, l = AnchorNames.Length; i < l; i++)
                CreateAnchor(trackingSpace, AnchorNames[i]);
        }

        private static void CreateAnchor(Transform trackingSpace, string name)
        {
            if (!trackingSpace.Find(name))
            {
                var gameObject = new GameObject(name);
                gameObject.transform.parent = trackingSpace;
            }
        }

#endregion
    }
}