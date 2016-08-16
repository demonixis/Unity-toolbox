/// GameVRSettings
/// Last Modified Date: 08/16/2016

using OSVR.Unity;
using UnityEngine;
using UnityEngine.VR;
using System.Collections;
using System;

namespace Demonixis.Toolbox.VR
{
    /// <summary>
    /// OsvrManager - Manages all aspect of the VR from this singleton.
    /// </summary>
    public sealed class OSVRManager : VRDeviceManager
    {
        private DisplayController displayController;

        [Tooltip("Add the name of the components you want to copy on the created cameras. Usefull for post processes.")]
        [SerializeField]
        private string[] _cameraComponentsToKeep = null;
        [Tooltip("Add the name of the components you want to remove from the main camera. Usefull for unused post processes.")]
        [SerializeField]
        private string[] _cameraComponentsToRemove = null;

        public override bool IsEnabled
        {
            get { return displayController != null; }
        }

        public override bool IsAvailable
        {
            get { return Detect; }
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

        public static bool Detect
        {
            get
            {
                var clientKit = ClientKit.instance;
                return clientKit != null && clientKit.context != null && clientKit.context.CheckStatus();
            }
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

        public override void Dispose()
        {
            if (displayController != null)
                SetVREnabled(false);

            Destroy(this);
        }

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
            var clientKit = ClientKit.instance;
            var camera = Camera.main;

            if (camera != null && clientKit != null && clientKit.context != null && clientKit.context.CheckStatus())
            {
                if (isEnabled && displayController == null)
                {
                    displayController = camera.transform.parent.gameObject.AddComponent<DisplayController>();
                    displayController.showDirectModePreview = showDirectModePreview;
                    camera.gameObject.AddComponent<VRViewer>();

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
        }

        private IEnumerator CopyComponentsToEyes()
        {
			var camera = Camera.main.transform;
			var i = 0;
			var size = 0;
			
			if (_cameraComponentsToKeep != null)
			{
				yield return new WaitForEndOfFrame();

				Component component = null;

				var eyes = camera.parent.GetComponentsInChildren<VRSurface>(true);

				size = _cameraComponentsToKeep.Length;

				for (i = 0; i < size; i++)
				{
					component = camera.GetComponent(_cameraComponentsToKeep[i]);

					if (component != null)
						for (int j = 0, eyeCount = eyes.Length; j < eyeCount; j++)
							CopyComponent(component, eyes[j].gameObject);
				}
			}
			
			size = _cameraComponentsToRemove.Length;
			
            for (i = 0; i < size; i++)
                Destroy(camera.GetComponent(_cameraComponentsToRemove[i]));
        }

        private void Destroy<T>(bool multiple = true) where T : MonoBehaviour
        {
            if (multiple)
            {
                var scripts = FindObjectsOfType<T>();
                for (int i = 0, l = scripts.Length; i < l; i++)
                    Destroy(scripts[i]);
            }
            else
                Destroy(FindObjectOfType<T>());
        }
    }
}