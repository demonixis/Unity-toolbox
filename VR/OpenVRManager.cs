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

            // /!\ You have to change one line in SteamVR_Camera.cs
            // var camera = head.GetComponent<Camera>();
            // become
            // var camera = head.gameObject.AddComponent<Camera>();
            // Because the head is just created and no scripts are attached to it.

            if (steamCamera == null)
            {
                var playerObject = GameObject.FindWithTag("Player");
                var camera = Camera.main.transform;
                var trackingSpace = camera.parent;

                if (playerObject == null || trackingSpace == null || trackingSpace.parent == null)
                    throw new UnityException("[OpenVRManager] Your prefab doesn't respect the correct hierarchy");


                // Creates the SteamVR's main camera.
                steamCamera = camera.gameObject.AddComponent<SteamVR_Camera>();

                // The controllers.
                var controllerObject = new GameObject("SteamVR_Controllers");
                var controllerTransform = controllerObject.transform;
                controllerTransform.parent = playerObject.transform;
                controllerTransform.localPosition = Vector3.zero;
                controllerTransform.localRotation = Quaternion.identity;
                // We need to disable the gameobject because the SteamVR_ControllerManager component
                // Will check attached controllers in the awake method.
                // Here we don't have attached controllers yet.
                controllerObject.SetActive(false);

                var controllerManager = controllerObject.AddComponent<SteamVR_ControllerManager>();
                controllerManager.left = CreateController(controllerManager.transform, "Controller (left)");
                controllerManager.right = CreateController(controllerManager.transform, "Controller (right)");

                // The controllers' model.
                if (_addModelControllers)
                {
                    CreateControllerModel(controllerManager.left.transform);
                    CreateControllerModel(controllerManager.right.transform);
                }

                // Now that controllers are attached, we can enable the GameObject
                controllerObject.SetActive(true);

                // And finally the play area.
                if (_addPlayArea)
                {
                    var playArea = new GameObject("SteamVR_PlayArea");
                    playArea.transform.parent = playerObject.transform;
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

        private GameObject CreateController(Transform parent, string name)
        {
            if (!parent.Find(name))
            {
                var anchor = new GameObject(name);
                anchor.transform.parent = parent;

                var controller = anchor.AddComponent<SteamVR_TrackedObject>();
                controller.index = SteamVR_TrackedObject.EIndex.None;
                return anchor;
            }

            return null;
        }

        private GameObject CreateControllerModel(Transform parent)
        {
            if (parent.GetComponentInChildren<SteamVR_RenderModel>() == null)
            {
                var model = new GameObject("Model");
                model.transform.parent = parent;

                var renderModel = model.AddComponent<SteamVR_RenderModel>();
                renderModel.index = SteamVR_TrackedObject.EIndex.None;
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