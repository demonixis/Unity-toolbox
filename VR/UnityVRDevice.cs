using System;
using UnityEngine;
using UnityEngine.VR;

namespace Demonixis.Toolbox.VR
{
    /// <summary>
    /// The UnityVRDevice is an abstract device that uses the UnityEngine.VR implementation.
    /// </summary>
    public abstract class UnityVRDevice : VRDeviceManager
    {
        #region Public Fields

        public override float RenderScale
        {
            get { return VRSettings.renderScale; }
            set { VRSettings.renderScale = value; }
        }

        public override VRDeviceType VRDeviceType
        {
            get { return VRDeviceType.UnityVR; }
        }

        public override Vector3 HeadPosition
        {
            get { return InputTracking.GetLocalPosition(VRNode.Head); }
        }

        #endregion

        public override void Recenter()
        {
            InputTracking.Recenter();
        }

        public override int CompareTo(object obj)
        {
            var other = obj as VRDeviceManager;

            if (other == null)
                return -1;

            // We use the default Unity's sorting
            if (other.UnityVRName != string.Empty)
            {
                var names = VRSettings.supportedDevices;
                var thisIndex = Array.IndexOf(names, UnityVRName);
                var otherIndex = Array.IndexOf(names, other.UnityVRName);

                if (otherIndex > thisIndex)
                    return -1;
                else
                    return 1;
            }

            return base.CompareTo(obj);
        }
    }
}
