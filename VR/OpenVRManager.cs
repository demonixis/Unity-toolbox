using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VR;
using Valve.VR;

namespace Demonixis.Toolbox.VR
{
    /// <summary>
    /// OsvrManager - Manages all aspect of the VR from this singleton.
    /// </summary>
    public sealed class OpenVRManager : VRDeviceManager
    {
        private SteamVR_Camera steamCamera = null;
        private SteamVR_ControllerManager controllerManager = null;

        #region Inspector Fields

        [SerializeField]
        private bool _addModelControllers = false;
        [SerializeField]
        private bool _addPlayArea = false;
        [SerializeField]
        private bool _fixHeadPosition = true;
        [SerializeField]
        private bool _fixHeadRotation = false;

        #endregion

        #region Public Fields

        public override bool IsEnabled
        {
            get { return VRSettings.enabled && OpenVR.IsHmdPresent(); }
        }

        public override bool IsPresent
        {
            get { return VRDevice.isPresent && OpenVR.IsHmdPresent(); }
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

        public override void SetVREnabled(bool isEnabled)
        {
            if (!IsPresent)
                return;

            if (steamCamera == null)
            {
                var playerObject = GameObject.FindWithTag("Player");
                var camera = Camera.main.GetComponent<Transform>();
                var trackingSpace = camera.parent;

                if (playerObject == null || trackingSpace == null || trackingSpace.parent == null)
                    throw new UnityException("[OpenVRManager] Your prefab doesn't respect the correct hierarchy");

                // The controllers.
                controllerManager = playerObject.AddComponent<SteamVR_ControllerManager>();

                // Creates the SteamVR's head
                /* Not sure if it's required because it works without that...
                var headCam = trackingSpace.gameObject.AddComponent<Camera>();
                headCam.clearFlags = CameraClearFlags.Nothing;
                headCam.cullingMask = 0;
                headCam.orthographic = true;
                headCam.orthographicSize = 1.0f;
                headCam.depth = 0.0f;
                headCam.useOcclusionCulling = false;
                headCam.hdr = false;

                var headTrackedObject = trackingSpace.gameObject.AddComponent<SteamVR_TrackedObject>();
                headTrackedObject.index = SteamVR_TrackedObject.EIndex.Hmd;
                */

                // Creates the SteamVR's main camera.
                steamCamera = camera.gameObject.AddComponent<SteamVR_Camera>();

                var player = playerObject.GetComponent<Transform>();
                controllerManager.left = CreateController(player, "Controller (left)", SteamVR_TrackedObject.EIndex.Device1);
                controllerManager.right = CreateController(player, "Controller (right)", SteamVR_TrackedObject.EIndex.Device2);

                // The controllers' model.
                if (_addModelControllers)
                {
                    CreateControllerModel(controllerManager.left.transform, SteamVR_TrackedObject.EIndex.Device1);
                    CreateControllerModel(controllerManager.right.transform, SteamVR_TrackedObject.EIndex.Device2);
                }

                // And finally the play area.
                if (_addPlayArea)
                {
                    var playArea = new GameObject("SteamVR_PlayArea");
                    playArea.transform.parent = player;

                    playArea.AddComponent<MeshRenderer>();
                    playArea.AddComponent<MeshFilter>();
                    playArea.AddComponent<SteamVR_PlayArea>();
                }

                StartCoroutine(FixHeadTransform(trackingSpace.parent, steamCamera.transform));
            }

            if (isEnabled)
                Recenter();

            VRSettings.enabled = isEnabled;
        }

        public override void Recenter()
        {
            InputTracking.Recenter();
        }

        private GameObject CreateController(Transform parent, string name, SteamVR_TrackedObject.EIndex index)
        {
            if (!parent.Find(name))
            {
                var anchor = new GameObject(name);
                anchor.transform.parent = parent;

                var controller = anchor.AddComponent<SteamVR_TrackedObject>();
                controller.index = index;
                return anchor;
            }

            return null;
        }

        private GameObject CreateControllerModel(Transform parent, SteamVR_TrackedObject.EIndex index)
        {
            if (parent.GetComponentInChildren<SteamVR_RenderModel>() == null)
            {
                var model = new GameObject("Model");
                model.transform.parent = parent;

                var renderModel = model.AddComponent<SteamVR_RenderModel>();
                renderModel.index = index;
            }

            return null;
        }

        private IEnumerator FixHeadTransform(Transform head, Transform mainCamera)
        {
            yield return new WaitForEndOfFrame();

            if (_fixHeadPosition)
            {
                var headPosition = head.localPosition;
                var camPosition = mainCamera.localPosition;
                headPosition.y -= camPosition.y;
                head.localPosition = headPosition;
            }

            if (_fixHeadRotation)
            {
                var camRotation = mainCamera.localRotation.eulerAngles;
                var headRotation = head.localRotation.eulerAngles;
                headRotation.y -= camRotation.y;
                head.localRotation = Quaternion.Euler(headRotation);
            }
        }
    }
}