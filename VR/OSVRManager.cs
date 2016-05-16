using OSVR.Unity;
using UnityEngine;

namespace Demonixis.Toolbox.VR
{
    /// <summary>
    /// OsvrManager - Manages all aspect of the VR from this singleton.
    /// </summary>
    public sealed class OSVRManager : VRDeviceManager
    {
        private DisplayController displayController;

        public override bool IsEnabled
        {
            get { return displayController != null; }
        }

        public override bool IsPresent
        {
            get
            {
                var clientKit = ClientKit.instance;
                return clientKit != null && clientKit.context != null && clientKit.context.CheckStatus();
            }
        }

        public override VRDeviceType VRDeviceType
        {
            get { return VRDeviceType.OSVR; }
        }

        public override float RenderScale
        {
            get { return 1.0f; }
            set { }
        }

        [SerializeField]
        private bool showDirectModePreview = true;

#if UNITY_EDITOR
        void Update()
        {
            if (displayController != null)
                displayController.showDirectModePreview = showDirectModePreview;
        }
#endif

        public override void Recenter()
        {
            var clientKit = ClientKit.instance;

            if (displayController != null && clientKit != null)
            {
                if (displayController.UseRenderManager)
                    displayController.RenderManager.SetRoomRotationUsingHead();
                else
                    clientKit.context.SetRoomRotationUsingHead();
            }
        }

        public void SetIPD(float ipd)
        {
            if (displayController != null && displayController.UseRenderManager)
                displayController.RenderManager.SetIPDMeters(ipd);
        }

        public override void SetVREnabled(bool isEnabled)
        {
            if (!IsPresent)
                return;

            var clientKit = ClientKit.instance;
            var camera = Camera.main;

            if (camera != null && clientKit != null && clientKit.context != null && clientKit.context.CheckStatus())
            {
                if (isEnabled)
                {
                    displayController = camera.transform.parent.gameObject.AddComponent<DisplayController>();
                    displayController.showDirectModePreview = showDirectModePreview;
                    camera.gameObject.AddComponent<VRViewer>();
                    Recenter();
                }
                else
                {
                    Destroy(displayController);

                    var viewers = FindObjectsOfType<VRViewer>();
                    for (int i = 0; i < viewers.Length; i++)
                        Destroy(viewers[i]);
                }
            }
        }
    }
}