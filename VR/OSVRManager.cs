using OSVR.Unity;
using UnityEngine;
using UnityEngine.VR;
using System.Collections;

namespace Demonixis.Toolbox.VR
{
    /// <summary>
    /// OsvrManager - Manages all aspect of the VR from this singleton.
    /// </summary>
    public sealed class OSVRManager : VRDeviceManager
    {
        private bool _unityVRWasEnabled = false;
        private DisplayController displayController;

        [Tooltip("Add the name of the components you want to copy on the created cameras. Usefull for post processes.")]
        [SerializeField]
        private string[] _componentsToCopyToEyes = null;
        [Tooltip("Add the name of the components you want to remove from the main camera. Usefull for unused post processes.")]
        [SerializeField]
        private string[] _componentsToRemoveFromCamera = null;

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

        public override void Dispose()
        {
            if (displayController != null)
            {
                SetVREnabled(false);
                VRSettings.enabled = _unityVRWasEnabled;
            }

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
            if (!IsPresent)
                return;

            _unityVRWasEnabled = VRSettings.enabled;
            VRSettings.enabled = false;

            var clientKit = ClientKit.instance;
            var camera = Camera.main;

            if (camera != null && clientKit != null && clientKit.context != null && clientKit.context.CheckStatus())
            {
                if (isEnabled && displayController == null)
                {
                    displayController = camera.transform.parent.gameObject.AddComponent<DisplayController>();
                    displayController.showDirectModePreview = showDirectModePreview;
                    camera.gameObject.AddComponent<VRViewer>();

                    if (_componentsToCopyToEyes != null)
                        StartCoroutine(CopyComponentsToEyes());

                    if (_componentsToRemoveFromCamera != null)
                        StartCoroutine(RemoveComponentsFromCamera());

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
            yield return new WaitForEndOfFrame();

            Component component = null;

            var camera = Camera.main.transform;
            var eyes = camera.parent.GetComponentsInChildren<VRSurface>(true);
            var eyesCount = eyes.Length;

            for (int i = 0, l = _componentsToCopyToEyes.Length; i < l; i++)
            {
                component = camera.GetComponent(_componentsToCopyToEyes[i]);

                if (component != null)
                {
                    for (int j = 0; j < eyesCount; j++)
                    {
                        if (CopyComponent(component, eyes[j].gameObject) != null)
                            Destroy(component);
                    }
                }
            }
        }

        private IEnumerator RemoveComponentsFromCamera()
        {
            yield return new WaitForEndOfFrame();

            var camera = Camera.main;
            for (int i = 0, l = _componentsToRemoveFromCamera.Length; i < l; i++)
                Destroy(camera.GetComponent(_componentsToRemoveFromCamera[i]));
        }

        private Component CopyComponent(Component component, GameObject destination)
        {
            var type = component.GetType();
            var copy = destination.AddComponent(type);
            var fields = type.GetFields();

            for (int i = 0, l = fields.Length; i < l; i++)
                fields[i].SetValue(copy, fields[i].GetValue(component));

            return copy;
        }

        private void Destroy<T>(bool multiple = true) where T : MonoBehaviour
        {
            if (multiple)
            {
                var scripts = FindObjectsOfType<T>();
                for (int i = 0; i < scripts.Length; i++)
                    Destroy(scripts[i]);
            }
            else
                Destroy(FindObjectOfType<T>());
        }
    }
}