/// UnityVRDevice
/// Last Modified Date: 01/07/2017

using UnityEngine;
using UnityEngine.VR;

namespace Demonixis.Toolbox.VR
{
    /// <summary>
    /// The UnityVRDevice is an abstract device that uses the UnityEngine.VR implementation.
    /// </summary>
    public class UnityVRDevice : VRDeviceBase
    {
        #region Public Fields

        public override float RenderScale
        {
            get { return VRSettings.renderScale; }
            set { VRSettings.renderScale = value; }
        }

        public override int EyeTextureWidth
        {
            get { return VRSettings.eyeTextureWidth; }
        }

        public override int EyeTextureHeight
        {
            get { return VRSettings.eyeTextureHeight; }
        }

        public override VRDeviceType VRDeviceType
        {
            get { return VRDeviceType.UnityVR; }
        }

        public override Vector3 HeadPosition
        {
            get { return InputTracking.GetLocalPosition(VRNode.Head); }
        }

        public override bool IsAvailable
        {
            get { return VRSettings.enabled; }
        }

        #endregion

        public override void Recenter()
        {
            InputTracking.Recenter();
        }

        public override void SetActive(bool active)
        {
            VRSettings.enabled = active;
        }
    }
}
