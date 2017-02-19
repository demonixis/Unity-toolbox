/// OSVRDevice
/// Last Modified Date: 01/07/2017
#define EMULATE_OSVR_API_
using OSVR.Unity;
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Demonixis.Toolbox.VR
{
    /// <summary>
    /// OSVRDevice - Manages all aspect of the VR from this singleton.
    /// </summary>
    public sealed class OSVRDevice : VRDeviceBase
    {
#if UNITY_STANDALONE
        private const string AppID = "net.demonixis.GunSpinningVR";
        private const bool AutoStartServer = false;
        private static ClientKit _clientKit = null;
        private DisplayController displayController = null;
#endif
        [SerializeField]
        private Transform _cameraNode = null;
        [SerializeField]
        private string[] _cameraComponentsToMove = null;

        public override bool IsAvailable
        {
            get
            {
#if UNITY_STANDALONE
                var clientKit = GetClientKit();
                return clientKit != null && clientKit.context != null && clientKit.context.CheckStatus();
#else
                return false;
#endif
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

        public override int EyeTextureWidth
        {
            get { return Screen.width; }
        }

        public override int EyeTextureHeight
        {
            get { return Screen.height; }
        }

        [SerializeField]
        private bool _showDirectModePreview = true;

#if UNITY_STANDALONE && UNITY_EDITOR
        private void Update()
        {
            if (displayController != null)
                displayController.showDirectModePreview = _showDirectModePreview;
        }
#endif

        public override void Dispose()
        {
#if UNITY_STANDALONE
            if (displayController != null)
                SetActive(false);
#endif
            Destroy(this);
        }

        public static ClientKit GetClientKit()
        {
#if UNITY_STANDALONE
            if (_clientKit == null)
            {
                var go = new GameObject("ClientKit");
                go.SetActive(false);

                _clientKit = go.AddComponent<ClientKit>();
                _clientKit.AppID = AppID;
#if UNITY_STANDALONE_WIN
                _clientKit.autoStartServer = false;
#endif
                go.SetActive(true);
            }

            return _clientKit;
#else
            return null;
#endif
        }

        public override void Recenter()
        {
#if UNITY_STANDALONE
            var clientKit = GetClientKit();

            if (clientKit != null)
            {
                if (displayController != null && displayController.UseRenderManager)
                    displayController.RenderManager.SetRoomRotationUsingHead();
                else
                    clientKit.context.SetRoomRotationUsingHead();
            }
#endif
        }

        public void SetIPD(float ipd)
        {
#if UNITY_STANDALONE
            if (displayController != null && displayController.UseRenderManager)
                displayController.RenderManager.SetIPDMeters(ipd);
#endif
        }

        public override void SetActive(bool isEnabled)
        {
#if UNITY_STANDALONE
            if (isEnabled)
                UnityEngine.VR.VRSettings.enabled = false;

            var clientKit = GetClientKit();
            var camera = Camera.main;

            if (camera != null && clientKit != null && clientKit.context != null && clientKit.context.CheckStatus())
            {
                if (isEnabled && displayController == null)
                {
                    displayController = camera.transform.parent.gameObject.AddComponent<DisplayController>();
                    displayController.showDirectModePreview = _showDirectModePreview;
                    camera.gameObject.AddComponent<VRViewer>();

                    if (_cameraNode != null && _cameraComponentsToMove != null && _cameraComponentsToMove.Length > 0)
                        StartCoroutine(CopyComponentsToEyes());
                }
                else if (displayController != null)
                {
                    Destroy(displayController);
                    Destroy<VREye>();
                    Destroy<VRSurface>();
                    Destroy<VRViewer>();
                    Destroy<OsvrRenderManager>(false);
                    displayController = null;
                }
            }
#endif
        }

        private IEnumerator CopyComponentsToEyes()
        {
            yield return new WaitForEndOfFrame();
        }
    }
}

#region OSVR API Emulation 

#if EMULATE_OSVR_API
namespace OSVR.Unity
{
    public class ClientKit : MonoBehaviour
    {
        public class Context
        {
            public bool CheckStatus() { return false; }
            public void SetRoomRotationUsingHead() { }
        }

        public bool autoStartServer;
        public string AppID = "APPID";
        public Context context;
    }

    public class DisplayController : MonoBehaviour
    {
        public bool showDirectModePreview = false;
        public bool UseRenderManager = false;
        public OsvrRenderManager RenderManager;
    }

    public class VREye : MonoBehaviour { }
    public class VRSurface : MonoBehaviour { }
    public class VRViewer : MonoBehaviour { }
    public class OsvrRenderManager : MonoBehaviour
    {
        public void SetRoomRotationUsingHead() { }
        public void SetIPDMeters(float ipd) { }
    }
}
#endif

#endregion