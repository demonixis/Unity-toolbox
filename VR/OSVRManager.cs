using OSVR.Unity;
using System.Collections;
using UnityEngine;

namespace Demonixis.Toolbox.VR
{
    /// <summary>
    /// OsvrManager - Manages all aspect of the VR from this singleton.
    /// </summary>
    public sealed class OsvrManager : VRManager
    {
        private static OsvrManager _instance = null;
        private DisplayController displayController;

        [SerializeField]
        private bool showDirectModePreview = true;

        #region Singleton

        public static OsvrManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<OsvrManager>();

                    if (_instance == null)
                    {
                        var go = new GameObject("OsvrManager");
                        _instance = go.AddComponent<OsvrManager>();
                    }
                }

                return _instance;
            }
        }

        #endregion

#if UNITY_EDITOR
        void Update()
        {
            if (displayController != null)
                displayController.showDirectModePreview = showDirectModePreview;
        }
#endif

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

        public void SetIPD(float ipd)
        {
            var displayController = Instance.displayController;

            if (displayController != null && displayController.UseRenderManager)
                displayController.RenderManager.SetIPDMeters(ipd);
        }

        public override void SetVREnabled(bool isEnabled)
        {
            var clientKit = ClientKit.instance;
            var camera = Camera.main;

            if (camera != null && clientKit != null && clientKit.context != null && clientKit.context.CheckStatus())
            {
                if (isEnabled)
                {
                    displayController = camera.transform.parent.gameObject.AddComponent<DisplayController>();
                    displayController.showDirectModePreview = _instance.showDirectModePreview;
                    camera.gameObject.AddComponent<VRViewer>();
                    StartCoroutine(RecenterView());
                }
                else
                {
                    Destroy(_instance.displayController);

                    var viewers = FindObjectsOfType<VRViewer>();
                    for (int i = 0; i < viewers.Length; i++)
                        Destroy(viewers[i]);
                }

                vrEnabled = isEnabled;
            }
        }

        #region Static Methods

        public static Vector3 GetLocalPosition(byte viewerIndex)
        {
            var displayController = Instance.displayController;
            if (displayController != null && displayController.DisplayConfig != null)
            {
                var pose3 = displayController.DisplayConfig.GetViewerPose(viewerIndex);
                return new Vector3((float)pose3.translation.x, (float)pose3.translation.y, (float)pose3.translation.z);
            }

            return Vector3.zero;
        }

        public static Quaternion GetLocalRotation(uint viewerIndex)
        {
            var displayController = Instance.displayController;
            if (displayController != null && displayController.DisplayConfig != null)
            {
                var pose3 = displayController.DisplayConfig.GetViewerPose(viewerIndex);
                return new Quaternion((float)pose3.rotation.x, (float)pose3.rotation.y, (float)pose3.rotation.z, (float)pose3.rotation.w);
            }

            return Quaternion.identity;
        }

        public static void Recenter()
        {
            var manager = Instance;
            var clientKit = ClientKit.instance;
            var displayController = manager.displayController;

            if (displayController != null && clientKit != null)
            {
                if (displayController.UseRenderManager)
                    displayController.RenderManager.SetRoomRotationUsingHead();
                else
                    clientKit.context.SetRoomRotationUsingHead();
            }
        }

        public static bool IsServerConnected()
        {
            var clientKit = ClientKit.instance;
            return clientKit != null && clientKit.context != null && clientKit.context.CheckStatus();
        }

        public static void SetRenderScale(float scale)
        {
            Debug.Log("[OsvrManager] SetRenderScale not yet supported");
        }

        #endregion
    }
}